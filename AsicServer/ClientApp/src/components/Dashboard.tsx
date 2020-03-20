import * as React from 'react';
import { connect } from 'react-redux';
import { Button, message, Modal, Row, Input, Col, Divider, Upload, Icon, Table } from 'antd'
import { RouteComponentProps } from 'react-router';
import Form, { FormComponentProps } from 'antd/lib/form';
import { isNullOrUndefined } from 'util';
import { parse } from 'papaparse';
import { renderStripedTable } from '../utils';

interface DashboardComponentState {
  modalVisible: boolean,
  attendees: [],
  page: number
}

type Props = FormComponentProps
  & RouteComponentProps<{}>; // ... plus incoming routing parameters

class Dashboard extends React.PureComponent<Props, DashboardComponentState> {

  constructor(props: any) {
    super(props);
    this.state = {
      modalVisible: false,
      attendees: [],
      page: 1
    }
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
      }
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
              attendees: results.data
            }, () => {
              resolve();
            });
          } else {
            message.error("You upload a csv file with wrong format. Please try again!", 3);
            thisState.setState({
              attendees: []
            }, () => { reject(); });
          }
        }, error: function (errors: any, file: File) {
          message.error("Upload error: " + errors, 3);
          thisState.setState({
            attendees: []
          }, () => { reject(); });
        }
      });
    });
  }

  private checkValidFileFormat = (attendees: []) => {
    let temp: { Code: string }[] = attendees;
    if (temp.length > 0) {
      if (!isNullOrUndefined(temp[0].Code)) {
        console.log('true');
        return true;
      }
    }
    console.log('false');
    return false;
  }

  private onPageChange = (page: number) => {
    this.setState({
      page: page
    })
  }

  public render() {
    const { getFieldDecorator } = this.props.form;
    const columns = [
      {
        title: "#",
        key: "index",
        width: '5%',
        render: (text: any, record: any, index: number) => (this.state.page - 1) * 5 + index + 1
      },
      {
        title: 'Code',
        key: 'Code',
        dataIndex: 'Code'
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
            <Table dataSource={this.state.attendees}
              columns={columns} rowKey="code"
              bordered
              rowClassName={renderStripedTable}
              pagination={{
                pageSize: 5,
                total: this.state.attendees != undefined ? this.state.attendees.length : 0,
                showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} attendees`,
                onChange: this.onPageChange
              }}
            />;
        </Row>
        </Modal>
      </React.Fragment>
    );
  }
}
export default Form.create({ name: 'create_dataset' })(connect()(Dashboard));
