import * as React from 'react';
import { connect } from 'react-redux';
import { Button, message, Modal, Row, Input, Col, Divider, Upload, Icon, Table } from 'antd'
import { RouteComponentProps } from 'react-router';
import NewUser from './NewUser';
import { UserState } from '../store/user/userState';
import { userActionCreators } from '../store/user/userActionCreators';
import { bindActionCreators } from 'redux';
import { ApplicationState } from '../store';

interface TrainModelState {

}

type Props = 
    UserState // ... state we've requested from the Redux store
    & typeof userActionCreators // ... plus action creators we've requested
    & RouteComponentProps<{}>; // ... plus incoming routing parameters

class TrainModel extends React.PureComponent<Props, TrainModelState> {

  constructor(props: Props) {
    super(props);
  }

  public render() {
    return (
        <React.Fragment>
      </React.Fragment>
    );
  }
}

const matchDispatchToProps = (dispatch: any) => {
  return bindActionCreators(userActionCreators, dispatch);
}

export default connect((state: ApplicationState) =>
    ({ ...state.user }), matchDispatchToProps)(TrainModel);
