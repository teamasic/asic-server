import * as React from 'react';
import { Route, Redirect, withRouter, RouteComponentProps } from 'react-router';
import Layout from './components/Layout';
import './App.css';
import { constants } from './constants/constant';
import Dashboard from './components/Dashboard';
import 'firebase/auth';
import { routes } from './constants/routes';
import Login from './components/Login';
import { connect } from 'react-redux';
import { UserState } from './store/user/userState';
import User from './models/User';
import { userActionCreators } from './store/user/userActionCreators';
import { bindActionCreators } from 'redux';
import { ApplicationState } from './store';
import TrainModel from './components/TrainModel';

type AppProps=
    UserState &
    typeof userActionCreators &
    RouteComponentProps<{}>; // ... plus incoming routing parameters

class AppComponent extends React.Component<AppProps> {

    constructor(props: any) {
        super(props);
        if (!this.props.isLogin) {
            this.props.checkUserInfo();
        }
    }

    componentDidMount() {
        if (!this.props.isLogin) {
            this.props.checkUserInfo();
        }
    }

    public render() {
        if (!this.props.successfullyLoaded) {
            return (
                <Layout>
                </Layout>
            );
        }
        if (this.props.isLogin) {
            console.log(this.props.currentUser);
            return (
                <Layout>
                    <Route exact path={routes.DASHBOARD} component={Dashboard} />
                    <Route exact path={routes.TRAIN_MODEL} component={TrainModel} />
                    <Route exact path="/" component={Dashboard} />
                </Layout>
            );
        } else {
            return (
                <Layout>
                    <Redirect exact to={routes.DEFAULT} />
                    <Route exact path={routes.DEFAULT} component={Login} />
                </Layout>
            );
        }
    }
}


const matchDispatchToProps = (dispatch: any) => {
    return bindActionCreators(userActionCreators, dispatch);
  }

export default withRouter(connect((state: ApplicationState) =>
    state.user, matchDispatchToProps)(AppComponent));




