import * as React from 'react';
import { connect } from 'react-redux';
import { Button, message, Modal, Row, Input, Col, Divider, Typography, Upload, Icon, Table, Spin, Breadcrumb, Result, Card } from 'antd'
import { RouteComponentProps } from 'react-router';
import { ModelState } from '../store/model/state';
import { modelActionCreators } from '../store/model/actionCreators';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';
import { formatDateDDMMYYYYHHmm } from '../utils';
import '../styles/TrainModel.css';

const { Title, Paragraph } = Typography;

interface TrainModelState {

}

type Props = 
    ModelState // ... state we've requested from the Redux store
    & typeof modelActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class TrainModel extends React.PureComponent<Props, TrainModelState> {

    constructor(props: Props) {
        super(props);
    }

    componentDidMount() {
        this.props.getLastTrainResult();
        this.props.checkIsTraining();
    }

    public train() {
        this.props.train();
    }

    public render() {
        return (
            <React.Fragment>
                <div className="breadcrumb-container">
                    <Breadcrumb>
                        <Breadcrumb.Item>
                            <Icon type="home" />
                        </Breadcrumb.Item>
                        <Breadcrumb.Item>
                            <span>Train model</span>
                        </Breadcrumb.Item>
                    </Breadcrumb>
                </div>
                <div className="title-container">
                    <Title className="title" level={3}>Train Model</Title>
                </div>
                {
                    this.renderLastTrainResult()
                }
                <div className="train-container">
                    {
                        this.props.isTraining ?
                            <Result
                            icon={<Spin size="large" />}
                            title="Training..."
                                extra={
                                    <div>
                                        <Button disabled={true} type="primary">Start training</Button>
                                        <div className="small">This operation might take a while.</div>
                                    </div>
                                }
                            /> : <Result
                                icon={<Icon type="reconciliation" theme="twoTone" />}
                                title="Start training facial recognition model"
                                extra={
                                    <div>
                                        <Button type="primary" onClick={() => this.train()}>Start training</Button>
                                        <div className="small">This operation might take a while.</div>
                                    </div>
                                }
                            />
                    }
                </div>
            </React.Fragment>
        );
    }

    renderLastTrainResult() {
        if (this.props.lastTrainResult == null) {
            return <></>;
        }
        const lastTrainResult = this.props.lastTrainResult;
        return <Card className="last-train-result"
            title="Most recent train result"
            size="small">
                <div className="item">
                    <div className="label">Last trained:</div>
                    <div className="result">
                        {formatDateDDMMYYYYHHmm(lastTrainResult.timeFinished)}
                    </div>
                </div>
                <div className="item">
                    <div className="label">Number of attendees:</div>
                    <div className="result">{lastTrainResult.attendeeCount}</div>
                </div>
                <div className="item">
                    <div className="label">Number of images:</div>
                    <div className="result">{lastTrainResult.imageCount}</div>
                </div>
            </Card>;
    }
}

const matchDispatchToProps = (dispatch: any) => {
    return bindActionCreators(modelActionCreators, dispatch);
}

export default connect((state: ApplicationState) =>
    ({ ...state.model }), matchDispatchToProps)(TrainModel);
