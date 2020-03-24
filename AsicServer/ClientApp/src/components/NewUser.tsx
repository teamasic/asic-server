import * as React from 'react';
import { connect } from 'react-redux';
import { Card, Upload, Button, Icon, Row, Col, Table, Form, Input } from 'antd'
import { renderStripedTable } from '../utils';
import Text from 'antd/lib/typography/Text';
import { parse } from 'papaparse';
import { isNullOrUndefined } from 'util';
import { userActionCreators } from '../store/user/userActionCreators';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';

const { Dragger } = Upload;

const tabList = [
    {
        key: 'multiple',
        tab: 'Multiple users',
    },
    {
        key: 'single',
        tab: 'Single user',
    },
];

const TAB_KEY = {
    MULTIPLE: 'multiple',
    SINGLE: 'single'
};

const ERR_MESSAGE = {
    INVALID_CSV_FILE_FORMAT: 'Only accept csv file format!',
    REQUIRED_CSV_FILE: 'Please import list users!',
    INVALID_ZIP_FILE_FORMAT: 'Only accept zip file format!',
    REQUIRED_ZIP_FILE: 'Please import zip file!',
    INVALID_CONTENT_FORMAT: 'You upload a csv file with wrong format. Please try again!',
    ERROR: 'Upload error: '
}

const FILE_TYPE = {
    CSV: 'application/vnd.ms-excel',
    ZIP: 'application/zip'
}

interface ComponentState {
    tabKey: string,
    importUsers: any,
    page: number,
    msgImportCSV: string,
    msgImportZIP: string,
    uploadingZIPFile: boolean,
    zipFile: File
}

type Props = typeof userActionCreators; // ... plus action creators we've requested

class NewUser extends React.PureComponent<Props, ComponentState> {

    constructor(props: Props) {
        super(props);
        this.state = {
            tabKey: 'multiple',
            importUsers: [],
            page: 1,
            msgImportCSV: ' ',
            msgImportZIP: ' ',
            uploadingZIPFile: false,
            zipFile: new File([], 'Null')
        }
    }

    private onTabChange = (key: string) => {
        console.log(key);
        this.setState({ tabKey: key })
    };

    private onPageChange = (page: number) => {
        this.setState({
            page: page
        })
    }

    private validateBeforeUpload = (file: File, fileType: string): boolean | Promise<void> => {
        if (file.type !== fileType) {
            switch (fileType) {
                case FILE_TYPE.CSV:
                    this.setState({ msgImportCSV: ERR_MESSAGE.INVALID_CSV_FILE_FORMAT });
                    break;
                case FILE_TYPE.ZIP:
                    this.setState({ msgImportZIP: ERR_MESSAGE.INVALID_ZIP_FILE_FORMAT });
                    break;
            }
            return false;
        }
        if (fileType === FILE_TYPE.ZIP) {
            this.setState({ zipFile: file });
            return true
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
                            importUsers: results.data,
                            msgImportCSV: ''
                        }, () => {
                            resolve();
                        });
                    } else {
                        thisState.setState({
                            importUsers: [],
                            msgImportCSV: ERR_MESSAGE.INVALID_CONTENT_FORMAT
                        }, () => { reject(); });
                    }
                }, error: function (errors: any, file: File) {
                    thisState.setState({
                        importUsers: [],
                        msgImportCSV: ERR_MESSAGE.ERROR + errors
                    }, () => { reject(); });
                }
            });
        });
    }

    private checkValidFileFormat = (users: []) => {
        let temp: { Email: string, RollNumber: string, Fullname: string }[] = users;
        if (temp.length > 0) {
            if (!isNullOrUndefined(temp[0].Email)
                && !isNullOrUndefined(temp[0].RollNumber)
                && !isNullOrUndefined(temp[0].Fullname)) {
                return true;
            }
        }
        return false;
    }

    private onZipFileChange = (info: any) => {
        console.log(info);
        if (info.file.type === FILE_TYPE.ZIP) {
            var status = info.file.status;
            console.log(status);
            if (status === 'uploading') {
                this.setState({ uploadingZIPFile: true });
            } else {
                this.setState({
                    uploadingZIPFile: false,
                    msgImportZIP: ''
                });
            }
        }
    }

    private createMultipleUsers = () => {
        if (this.validateData()) {
            var users = this.state.importUsers;
            var importUsers = new Array(0);
            users.forEach((item: any) => {
                var user = {
                    email: item.Email,
                    username: item.Email,
                    fullname: item.Fullname,
                    rollnumber: item.RollNumber
                };
                importUsers.push(user);
            });
            var createUsers = {
                users: importUsers,
                zipFile: this.state.zipFile
            };
            console.log(createUsers);
            this.props.requestCreateUsers(createUsers);
        }
    }

    private validateData = () => {
        var result = true;
        if (this.state.importUsers.length === 0) {
            this.setState({ msgImportCSV: ERR_MESSAGE.REQUIRED_CSV_FILE });
            return false;
        }
        if (this.state.zipFile.name === 'Null') {
            this.setState({ msgImportZIP: ERR_MESSAGE.REQUIRED_ZIP_FILE });
            return false;
        }
        return result;
    }

    render() {
        return (
            <div>
                <Card
                    style={{ width: '100%' }}
                    title='Create user'
                    tabList={tabList}
                    activeTabKey={this.state.tabKey}
                    onTabChange={key => {
                        this.onTabChange(key);
                    }}
                >
                    {this.renderContent()}
                </Card>
            </div>
        );
    }

    private renderContent = () => {
        const columns = [
            {
                title: "#",
                key: "index",
                width: '5%',
                render: (text: any, record: any, index: number) => (this.state.page - 1) * 5 + index + 1
            },
            {
                title: 'Email',
                key: 'email',
                dataIndex: 'Email',
                width: '40%'
            },
            {
                title: 'Roll number',
                key: 'rollNumber',
                dataIndex: 'RollNumber',
                width: '25%'
            },
            {
                title: 'Fullname',
                key: 'fullname',
                dataIndex: 'Fullname',
                width: '30%'
            }
        ];
        return (
            this.state.tabKey === TAB_KEY.MULTIPLE ?
                (
                    <div>
                        <Row style={{ marginBottom: 10 }}>
                            <Col span={4}>
                                <Upload
                                    multiple={false}
                                    accept=".csv"
                                    showUploadList={false}
                                    beforeUpload={(file: File) => this.validateBeforeUpload(file, FILE_TYPE.CSV)}
                                >
                                    <Button><Icon type="upload" /> Upload CSV File </Button>
                                </Upload>
                            </Col>
                            <Col span={20}>
                                {this.state.msgImportCSV.length === 0 ?
                                    (<Icon type="check-circle" theme="twoTone" twoToneColor="#52c41a" />) :
                                    this.state.msgImportCSV.length !== 1 ?
                                        (<Icon type="close-circle" theme="twoTone" twoToneColor="#ff0000" />) : null
                                }
                                <Text type="danger"> {this.state.msgImportCSV}</Text>
                            </Col>
                        </Row>
                        <Row style={{ marginBottom: 10 }}>
                            <Table dataSource={this.state.importUsers}
                                columns={columns}
                                rowKey="Email"
                                bordered
                                rowClassName={renderStripedTable}
                                pagination={{
                                    pageSize: 10,
                                    total: this.state.importUsers !== undefined ? this.state.importUsers.length : 0,
                                    showTotal: (total: number, range: [number, number]) => `${range[0]}-${range[1]} of ${total} rows`,
                                    onChange: this.onPageChange
                                }}
                            />
                        </Row>
                        <Row style={{ marginBottom: 10 }}>
                            <Col span={8}>
                                <Dragger
                                    multiple={false}
                                    accept=".zip"
                                    showUploadList={false}
                                    beforeUpload={(file: File) => this.validateBeforeUpload(file, FILE_TYPE.ZIP)}
                                    onChange={this.onZipFileChange}
                                >
                                    <p className="ant-upload-drag-icon">
                                        {this.state.uploadingZIPFile == true ?
                                            (<Icon type="loading" />) :
                                            (<Icon type="inbox" />)
                                        }
                                    </p>
                                    <p className="ant-upload-text">Click or drag file to this area to upload</p>
                                    <p className="ant-upload-hint">Only support for .zip file</p>
                                </Dragger>
                            </Col>
                            <Col span={15} offset={1}>
                                {this.state.msgImportZIP.length === 0 ?
                                    (<Icon type="check-circle" theme="twoTone" twoToneColor="#52c41a" />) :
                                    this.state.msgImportZIP.length !== 1 ?
                                        (<Icon type="close-circle" theme="twoTone" twoToneColor="#ff0000" />) : null
                                }
                                <Text type="danger"> {this.state.msgImportZIP}</Text>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={8}>
                                <Button type="primary" style={{ width: '100%' }} onClick={this.createMultipleUsers}>Save</Button>
                            </Col>
                        </Row>
                    </div>
                ) :
                (
                    <div>
                        <Row style={{ marginBottom: 10 }}>
                            <Col span={14}>
                                <Form labelCol={{ span: 4 }} wrapperCol={{ span: 10 }}>
                                    <Form.Item label="Email" required>
                                        <Input />
                                    </Form.Item>
                                    <Form.Item label="Roll number" required>
                                        <Input />
                                    </Form.Item>
                                    <Form.Item label="Fullname" required>
                                        <Input />
                                    </Form.Item>
                                </Form>
                            </Col>
                        </Row>
                        <Row style={{ marginBottom: 10 }}>
                            <Col span={8}>
                                <Dragger
                                    multiple={false}
                                >
                                    <p className="ant-upload-drag-icon">
                                        <Icon type="inbox" />
                                    </p>
                                    <p className="ant-upload-text">Click or drag file to this area to upload</p>
                                    <p className="ant-upload-hint">Only support for .zip file</p>
                                </Dragger>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={8}>
                                <Button type="primary" style={{ width: '100%' }}>Save</Button>
                            </Col>
                        </Row>
                    </div>
                )
        );
    }
}

const matchDispatchToProps = (dispatch: any) => {
    return bindActionCreators(userActionCreators, dispatch);
}

export default connect((state: ApplicationState) => ({...state.user}), matchDispatchToProps)(NewUser);