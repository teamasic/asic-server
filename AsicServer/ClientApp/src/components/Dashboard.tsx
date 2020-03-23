import * as React from 'react';
import { connect } from 'react-redux';
import { Button, message, Modal, Row, Input, Col, Divider, Upload, Icon, Table } from 'antd'
import { RouteComponentProps } from 'react-router';
import Form, { FormComponentProps } from 'antd/lib/form';
import { isNullOrUndefined } from 'util';
import { parse } from 'papaparse';
import { renderStripedTable } from '../utils';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';

interface DashboardComponentState {
  modalVisible: boolean,
  rollNumbers: [],
  modalPageNo: number,
  dataSetPageNo: number,
  tblDataSetLoading: boolean
}

type Props = FormComponentProps
  & RouteComponentProps<{}>; // ... plus incoming routing parameters

class Dashboard extends React.PureComponent<Props, DashboardComponentState> {

  constructor(props: any) {
    super(props);
    this.state = {
      modalVisible: false,
      rollNumbers: [],
      modalPageNo: 1,
      dataSetPageNo: 1,
      tblDataSetLoading: true
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
          
      </React.Fragment>
    );
  }
}

export default Form.create({ name: 'create_dataset' })
(connect()(Dashboard));
