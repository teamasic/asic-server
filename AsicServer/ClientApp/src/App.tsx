import * as React from 'react';
import { Route, Redirect } from 'react-router';
import Layout from './components/Layout';
import './App.css';
import { constants } from './constants/constant';
import Dashboard from './components/Dashboard';
import 'firebase/auth';
import { routes } from './constants/routes';
import Login from './components/Login';

class AppComponent extends React.Component {

    constructor(props: any) {
        super(props);
    }

    // public componentDidMount() {
    //     firebase.auth.onAuthStateChanged(authUser => {
    //         authUser
    //             ? this.setState(() => ({ authUser }))
    //             : this.setState(() => ({ authUser: null }));
    //     });
    // }

    public render() {
        const authData = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
        if (authData) {
            return (
                <Layout >
                    <Route exact path={routes.DASHBOARD} component={Dashboard} />
                </Layout>
            );
        } else {
            return (
                <Layout >
                    <Route exact path={routes.DEFAULT} component={Login} />
                </Layout>
            );
        }
    }
}


export default AppComponent;




