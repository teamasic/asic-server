import * as React from 'react';
import { connect } from 'react-redux';
import { Card, Upload, Button, Icon, Row, Col, Table, Form, Input, Tooltip, Spin } from 'antd'
import { renderStripedTable } from '../utils';
import Text from 'antd/lib/typography/Text';
import { parse } from 'papaparse';
import { isNullOrUndefined } from 'util';
import { userActionCreators } from '../store/user/userActionCreators';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import { FormComponentProps } from 'antd/lib/form';
import User from '../models/User';

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
    msgImportMultipleZIP: string,
    msgImportSingleZIP: string,
    uploadingMultipleZIPFile: boolean,
    uploadingSingleZIPFile: boolean,
    multipleZipFile: File,
    singleZipFile: File,
    csvFile: File,
    user: any, 
    creatingMultipleUsers: boolean,
    creatingSingleUser: boolean
}

type Props = typeof userActionCreators // ... plus action creators we've requested
    & FormComponentProps;

class NewUser extends React.PureComponent<Props, ComponentState> {

    constructor(props: Props) {
        super(props);
        this.state = {
            tabKey: 'multiple',
            importUsers: [],
            page: 1,
            msgImportCSV: ' ',
            msgImportMultipleZIP: ' ',
            msgImportSingleZIP: ' ',
            uploadingMultipleZIPFile: false,
            uploadingSingleZIPFile: false,
            multipleZipFile: new File([], 'Null'),
            singleZipFile: new File([], 'Null'),
            csvFile: new File([], 'Null'),
            user: null,
            creatingMultipleUsers: false,
            creatingSingleUser: false
        }
    }

    private onTabChange = (key: string) => {
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
                    switch (this.state.tabKey) {
                        case TAB_KEY.MULTIPLE:
                            this.setState({ msgImportMultipleZIP: ERR_MESSAGE.INVALID_ZIP_FILE_FORMAT });
                            break;
                        case TAB_KEY.SINGLE:
                            this.setState({ msgImportSingleZIP: ERR_MESSAGE.INVALID_ZIP_FILE_FORMAT });
                            break;
                    }
                    break;
            }
            return false;
        }
        if (fileType === FILE_TYPE.ZIP) {
            switch (this.state.tabKey) {
                case TAB_KEY.MULTIPLE:
                    this.setState({ multipleZipFile: file });
                    break;
                case TAB_KEY.SINGLE:
                    this.setState({ singleZipFile: file });
                    break;
            }
            return true;
        }
        return this.parseFileToTable(file);
    }

    private parseFileToTable = (file: File): Promise<void> => {
        return new Promise((resolve, reject) => {
            var thisState = this;
            parse(file, {
                header: true,
                complete: function (results: any, file: File) {
                    if (thisState.checkValidCSVFileFormat(results.data)) {
                        thisState.setState({
                            importUsers: results.data,
                            csvFile: file,
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

    private checkValidCSVFileFormat = (users: []) => {
        let temp: { email: string, rollNumber: string, fullname: string, image: string }[] = users;
        if (temp.length > 0) {
            if (!isNullOrUndefined(temp[0].email)
                && !isNullOrUndefined(temp[0].rollNumber)
                && !isNullOrUndefined(temp[0].fullname)
                && !isNullOrUndefined(temp[0].image)) {
                return true;
            }
        }
        return false;
    }

    private onZipFileChange = (info: any) => {
        if (info.file.type === FILE_TYPE.ZIP) {
            var status = info.file.status;
            switch (this.state.tabKey) {
                case TAB_KEY.MULTIPLE:
                    if (status === 'uploading') {
                        this.setState({ uploadingMultipleZIPFile: true });
                    } else {
                        this.setState({
                            uploadingMultipleZIPFile: false,
                            msgImportMultipleZIP: ''
                        });
                    }
                    break;
                case TAB_KEY.SINGLE:
                    if (status === 'uploading') {
                        this.setState({ uploadingSingleZIPFile: true });
                    } else {
                        this.setState({
                            uploadingSingleZIPFile: false,
                            msgImportSingleZIP: ''
                        });
                    }
                    break;
            }
        }
    }

    private createMultipleUsers = () => {
        if (this.validateData()) {
            this.setState({creatingMultipleUsers: true});
            var users = this.state.importUsers;
            var importUsers = new Array(0);
            users.forEach((item: any) => {
                var user = {
                    email: item.Email,
                    username: item.Email,
                    fullname: item.Fullname,
                    rollnumber: item.RollNumber,
                    image: item.Image
                };
                importUsers.push(user);
            });
            var csvFile = this.state.csvFile;
            var zipFile = this.state.multipleZipFile;
            this.props.requestCreateMultipleUsers(zipFile, csvFile, this.resetUsersTable);
        }
    }

    private createSingleUser = (e: any) => {
        e.preventDefault();
        var isUpLoadZipFile = this.state.singleZipFile.name !== 'Null';
        if (!isUpLoadZipFile) {
            this.setState({ msgImportSingleZIP: ERR_MESSAGE.REQUIRED_ZIP_FILE });
        }
        this.props.form.validateFields((err: any, values: any) => {
            if (!err && isUpLoadZipFile) {
                this.setState({creatingSingleUser: true});
                var user = {
                    email: values.email,
                    rollNumber: values.rollNumber,
                    fullname: values.fullname,
                    image: values.image
                };
                var zipFile = this.state.singleZipFile;
                this.props.requestCreateSingleUser(zipFile, user, this.createSingleUserSuccess, this.createSingleUserWithError);
            }
        });
    }

    private createSingleUserSuccess = () => {
        this.setState({
            singleZipFile: new File([], 'Null'),
            creatingSingleUser: false
        });
        this.props.form.setFieldsValue({
            email: '',
            rollNumber: '',
            fullname: '',
            image: ''
        });
        this.setUser(null);
    }

    private createSingleUserWithError = () => {
        this.setState({creatingSingleUser: false});
    }

    private validateData = () => {
        var result = true;
        if (this.state.importUsers.length === 0) {
            this.setState({ msgImportCSV: ERR_MESSAGE.REQUIRED_CSV_FILE });
            result = false;
        }
        if (this.state.multipleZipFile.name === 'Null') {
            this.setState({ msgImportMultipleZIP: ERR_MESSAGE.REQUIRED_ZIP_FILE });
            result = false;
        }
        if (this.state.csvFile.name === 'Null') {
            this.setState({ msgImportCSV: ERR_MESSAGE.REQUIRED_CSV_FILE });
            result = false;
        }
        return result;
    }

    private onEmailBlur = (e: any) => {
        var email = e.target.value;
        if (email.length > 0) {
            this.props.requestUserByEmail(email, this.setUser);
        }
    }

    private setUser = (user: any) => {
        this.setState({
            user: user
        });
        if (!isNullOrUndefined(user)) {
            this.props.form.setFieldsValue({
                rollNumber: user.rollNumber,
                fullname: user.fullname,
                image: user.image
            });
        } else {
            this.props.form.setFieldsValue({
                rollNumber: '',
                fullname: '',
                image: ''
            });
        }
    }

    private resetUsersTable = (data: any) => {
        this.setState({ 
            importUsers: data,
            creatingMultipleUsers: false
        });
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
                dataIndex: 'email',
                width: '30%',
                render: (text: any, record: any, index: number) => {
                    if (isNullOrUndefined(record.noImageSaved)) {
                        return text;
                    }
                    return (
                        <span>
                            {record.noImageSaved > 0 ?
                                (<span>{text} <Tooltip title="Saved user successfully!">
                                    <Icon type="check-circle" theme="twoTone" twoToneColor="#52c41a" />
                                </Tooltip>
                                </span>) :
                                (<span>{text} <Tooltip title="User is saved without images">
                                    <Icon type="info-circle" theme="twoTone" twoToneColor="#faad14" />
                                </Tooltip>
                                </span>)}
                        </span>
                    );
                }
            },
            {
                title: 'Roll number',
                key: 'rollNumber',
                dataIndex: 'rollNumber',
                width: '20%'
            },
            {
                title: 'Fullname',
                key: 'fullname',
                dataIndex: 'fullname',
                width: '30%'
            },
            {
                title: 'Image',
                key: 'image',
                dataIndex: 'image',
                width: '15%',
                ellipsis: true
            }
        ];
        const { getFieldDecorator } = this.props.form;
        return (
            this.state.tabKey === TAB_KEY.MULTIPLE ?
                (
                    <Spin spinning={this.state.creatingMultipleUsers}>
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
                                    (
                                        <div>
                                            <Icon type="check-circle" theme="twoTone" twoToneColor="#52c41a" />
                                            <Text type="secondary"> {this.state.csvFile.name}</Text>
                                        </div>
                                    ) :
                                    this.state.msgImportCSV.length !== 1 ?
                                        (<Icon type="close-circle" theme="twoTone" twoToneColor="#ff0000" />) : null
                                }
                                <Text type="danger"> {this.state.msgImportCSV}</Text>
                            </Col>
                        </Row>
                        <Row style={{ marginBottom: 10 }}>
                            <Table dataSource={this.state.importUsers}
                                columns={columns}
                                rowKey="email"
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
                                        {this.state.uploadingMultipleZIPFile === true ?
                                            (<Icon type="loading" />) :
                                            (<Icon type="inbox" />)
                                        }
                                    </p>
                                    <p className="ant-upload-text">Click or drag file to this area to upload</p>
                                    <p className="ant-upload-hint">Only support for .zip file</p>
                                </Dragger>
                            </Col>
                            <Col span={15} offset={1}>
                                {this.state.msgImportMultipleZIP.length === 0 ?
                                    (
                                        <div>
                                            <Icon type="check-circle" theme="twoTone" twoToneColor="#52c41a" />
                                            <Text type="secondary"> {this.state.multipleZipFile.name}</Text>
                                        </div>
                                    ) :
                                    this.state.msgImportMultipleZIP.length !== 1 ?
                                        (<Icon type="close-circle" theme="twoTone" twoToneColor="#ff0000" />) : null
                                }
                                <Text type="danger"> {this.state.msgImportMultipleZIP}</Text>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={8}>
                                <Button type="primary" style={{ width: '100%' }} onClick={this.createMultipleUsers}>Save</Button>
                            </Col>
                        </Row>
                    </Spin>
                ) :
                (
                    <Spin spinning={this.state.creatingSingleUser}>
                        <Row style={{ marginBottom: 10 }}>
                            <Col span={14} offset={4}>
                                <Form labelCol={{ span: 8 }} wrapperCol={{ span: 12 }}>
                                    <Form.Item label="Email" required>
                                        {getFieldDecorator('email', {
                                            rules: [
                                                { required: true, message: 'Please input email' },
                                                { type: 'email', message: 'The input is not valid E-mail' }
                                            ],
                                        })(
                                            <Input type="email" onBlur={this.onEmailBlur} />
                                        )}
                                    </Form.Item>
                                    <Form.Item label="Roll number" required>
                                        {getFieldDecorator('rollNumber', {
                                            rules: [
                                                { required: true, message: 'Please input roll number' },
                                                { min: 3, max: 10, message: 'Roll number requires 3-10 characters' }
                                            ],
                                        })(
                                            <Input type="text" disabled={this.state.user != null} />
                                        )}
                                    </Form.Item>
                                    <Form.Item label="Fullname" required>
                                        {getFieldDecorator('fullname', {
                                            rules: [
                                                { required: true, message: 'Please input fullname' },
                                                { min: 3, max: 50, message: 'Fullname requires 3-50 characters' }
                                            ],
                                        })(
                                            <Input type="text" disabled={this.state.user != null} />
                                        )}
                                    </Form.Item>
                                    <Form.Item label="Image" required>
                                        {getFieldDecorator('image', {
                                            rules: [
                                                { required: true, message: 'Please input image' },
                                                { min: 3, max: 100, message: 'Fullname requires 3-100 characters' }
                                            ],
                                        })(
                                            <Input type="text" disabled={this.state.user != null} />
                                        )}
                                    </Form.Item>
                                </Form>
                            </Col>
                        </Row>
                        <Row style={{ marginBottom: 10 }}>
                            <Col span={8} offset={8}>
                                <Dragger
                                    multiple={false}
                                    accept=".zip"
                                    showUploadList={false}
                                    beforeUpload={(file: File) => this.validateBeforeUpload(file, FILE_TYPE.ZIP)}
                                    onChange={this.onZipFileChange}
                                >
                                    <p className="ant-upload-drag-icon">
                                        {this.state.uploadingSingleZIPFile === true ?
                                            (<Icon type="loading" />) :
                                            (<Icon type="inbox" />)
                                        }
                                    </p>
                                    <p className="ant-upload-text">Click or drag file to this area to upload</p>
                                    <p className="ant-upload-hint">Only support for .zip file</p>
                                </Dragger>
                            </Col>
                            <Col span={7} offset={1}>
                                {this.state.msgImportSingleZIP.length === 0 ?
                                    (
                                        <div>
                                            <Icon type="check-circle" theme="twoTone" twoToneColor="#52c41a" />
                                            <Text type="secondary"> {this.state.singleZipFile.name}</Text>
                                        </div>
                                    ) :
                                    this.state.msgImportSingleZIP.length !== 1 ?
                                        (<Icon type="close-circle" theme="twoTone" twoToneColor="#ff0000" />) : null
                                }
                                <Text type="danger"> {this.state.msgImportSingleZIP}</Text>
                            </Col>
                        </Row>
                        <Row>
                            <Col span={8} offset={8}>
                                <Button type="primary" style={{ width: '100%' }}
                                    onClick={this.createSingleUser}>Save</Button>
                            </Col>
                        </Row>
                    </Spin>
                )
        );
    }
}

const matchDispatchToProps = (dispatch: any) => {
    return bindActionCreators(userActionCreators, dispatch);
}

export default Form.create({ name: 'create_single_use' })
    (connect((state: ApplicationState) => ({ ...state.user }), matchDispatchToProps)(NewUser));