import * as React from 'react';
import { connect } from 'react-redux';
import { Button, message, Modal, Row, Input, Col, Divider, Upload, Icon, Table } from 'antd'
import { RouteComponentProps } from 'react-router';
import Form, { FormComponentProps } from 'antd/lib/form';
import { isNullOrUndefined } from 'util';
import { parse } from 'papaparse';
import { renderStripedTable } from '../utils';
import { dataSetActionCreators } from '../store/dataset/dataSetActionCreator'
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import DataSet from '../models/DataSet';

interface DashboardComponentState {
  modalVisible: boolean,
  rollNumbers: [],
  dataSets: DataSet[],
  modalPageNo: number,
  dataSetPageNo: number,
  tblDataSetLoading: boolean
}

type Props = FormComponentProps
  & typeof dataSetActionCreators // ... plus action creators we've requested
  & RouteComponentProps<{}>; // ... plus incoming routing parameters

class Dashboard extends React.PureComponent<Props, DashboardComponentState> {

  constructor(props: any) {
    super(props);
    this.state = {
      modalVisible: false,
      rollNumbers: [],
      dataSets: new Array(0),
      modalPageNo: 1,
      dataSetPageNo: 1,
      tblDataSetLoading: true
    }
  }

  componentDidMount() {
    this.fetchData();
  }

  private openModal = () => {
    this.setState({
      modalVisible: true
    });
  }

  private closeModal = () => {
    this.setState({
      modalVisible: false
    });
  }

  private handleOk = (e: any) => {
    e.preventDefault();
    this.props.form.validateFields((err: any, values: any) => {
      if (!err) {
        console.log(values.name);
        var createDataSet = {
          id: 0,
          name: values.name,
          rollNumbers: this.state.rollNumbers
        };
        this.props.createDataSet(createDataSet, this.createDataSetSuccess);
      }
    });
  }

  private createDataSetSuccess = () => {
    this.fetchData();
    this.closeModal();
  }

  private setDataSets = (data: DataSet[]) => {
    this.setState({
      dataSets: data,
      tblDataSetLoading: false
    });
  }

  private validateBeforeUpload = (file: File): boolean | Promise<void> => {
    if (file.type !== "application/vnd.ms-excel") {
      //Show error in 3 second
      message.error("Only accept CSV file!", 3);
      return false;
    }
    return this.parseFileToTable(file);
  }

  private parseFileToTable = (file: File): Promise<void> => {
    return new Promise((resolve, reject) => {
      var thisState = this;
      parse(file, {
        header: true,
        complete: function (results: any, file: File) {
          if (thisState.checkValidFileFormat(results.data)) {
            thisState.setState({
              rollNumbers: results.data
            }, () => {
              resolve();
            });
          } else {
            message.error("You upload a csv file with wrong format. Please try again!", 3);
            thisState.setState({
              rollNumbers: []
            }, () => { reject(); });
          }
        }, error: function (errors: any, file: File) {
          message.error("Upload error: " + errors, 3);
          thisState.setState({
            rollNumbers: []
          }, () => { reject(); });
        }
      });
    });
  }

  private checkValidFileFormat = (attendees: []) => {
    let temp: { RollNumber: string }[] = attendees;
    if (temp.length > 0) {
      if (!isNullOrUndefined(temp[0].RollNumber)) {
        console.log('true');
        return true;
      }
    }
    console.log('false');
    return false;
  }

  private onModalPageChange = (page: number) => {
    this.setState({
      modalPageNo: page
    })
  }

  private onDataSetPageChange = (page: number) => {
    this.setState({
      dataSetPageNo: page
    })
  }

  public render() {
    const { getFieldDecorator } = this.props.form;
    const columns = [
      {
        title: "#",
        key: "index",
        width: '5%',
        render: (text: any, record: any, index: number) => (this.state.modalPageNo - 1) * 10 + index + 1
      },
      {
        title: 'Roll number',
        key: 'rollNumber',
        dataIndex: 'RollNumber'
      }
    ];
    const dataSetColumn = [
      {
        title: "#",
        key: "index",
        width: '5%',
        render: (text: any, record: any, index: number) => (this.state.dataSetPageNo - 1) * 10 + index + 1
      },
      {
        title: 'Name',
        key: 'name',
        width: '45%',
        dataIndex: 'name'
      },
      {
        title: 'No. Users',
        key: 'dataSetUser',
        width: '45%',
        render: (text: any, record: any, index: number) => record.dataSetUser.length + ' users'
      },
      {
        witdh: '5%',
        render: () => (<Button type="default" size="small">Detail</Button>)
      }
    ];
    return (
      <React.Fragment>
        <Button type="primary" onClick={this.openModal}>New Data Set</Button>
        <Modal
          visible={this.state.modalVisible}
          title="Create new Data set"
          centered
          onCancel={this.closeModal}
          width='50%'
          okText="Save"
          onOk={this.handleOk}
        >
          <Form layout="inline">
            <Row>
              <Col span={16}>
                <Form.Item label="Name" required>
                  {getFieldDecorator('name', {
                    rules: [{ required: true, message: 'Please input data set name' }],
                  })(
                    <Input placeholder="Enter name" />
                  )}
                </Form.Item>
              </Col>
            </Row>
          </Form>
          <Divider orientation="left">List Attendees</Divider>
          <Row gutter={[0, 32]}>
            <Col span={8}>
              <Upload
                multiple={false}
                accept=".csv"
                showUploadList={false}
                beforeUpload={this.validateBeforeUpload}
              >
                <Button>
                  <Icon type="upload" /> Upload CSV File
                </Button>
              </Upload>
            </Col>
          </Row>
          <Row>
            <Table dataSource={this.state.rollNumbers}
              columns={columns} 
              rowKey="RollNumber"
              bordered
              rowClassName={renderStripedTable}
              pagination={{
                pageSize: 10,
                total: this.state.rollNumbers != undefined ? this.state.rollNumbers.length : 0,
                showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} attendees`,
                onChange: this.onModalPageChange
              }}
            />;
        </Row>
        </Modal>
        <Table 
          dataSource={this.state.dataSets}
          columns={dataSetColumn} 
          rowKey="id"
          bordered
          loading={this.state.tblDataSetLoading}
          rowClassName={renderStripedTable}
          pagination={{
            pageSize: 10,
            total: this.state.dataSets != undefined ? this.state.dataSets.length : 0,
            showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} rows`,
            onChange: this.onDataSetPageChange
          }}
        />
      </React.Fragment>
    );
  }

  private fetchData = () => {
    this.setState({
      tblDataSetLoading: true
    });
    this.props.getAllDataSet(this.setDataSets);
  }
}

const matchDispatchToProps = (dispatch: any) => {
  return bindActionCreators(dataSetActionCreators, dispatch);
}

export default Form.create({ name: 'create_dataset' })
(connect((state: ApplicationState) => state.dataSet, matchDispatchToProps)(Dashboard));
