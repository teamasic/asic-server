import * as React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon, Divider, Row, Col } from 'antd';
import '../styles/Layout.css';
import { constants } from '../constants/constant';
import * as firebase from '../firebase';
import { UserState } from '../store/user/userState';
import { userActionCreators } from '../store/user/userActionCreators';
import { ApplicationState } from '../store';
import { bindActionCreators } from 'redux';
import { connect } from 'react-redux';
import { Link, withRouter } from 'react-router-dom';
import MenuBar from './MenuBar';

const { Header, Sider, Content, Footer } = Layout;
type LayoutProps =
	UserState &
	typeof userActionCreators; // ... plus incoming routing parameters

class PageLayout extends React.Component<
	LayoutProps,
	{
		collapsed: boolean;
	}
	> {
	state = {
		collapsed: false
	};

	constructor(props: LayoutProps) {
		super(props);
	}

	onCollapse = (collapsed: boolean) => {
		this.setState({ collapsed });
	};

	render() {
		return (<>{this.props.isLogin && this.props.successfullyLoaded ? this.renderLayout() : this.renderEmty()}</>);
	}
	private renderLayout() {
		return (
			<Layout className="layout">
				<Sider
					className="sider"
					collapsible
					collapsed={this.state.collapsed}
					onCollapse={this.onCollapse}
				>
					<div className="logo">ASIC</div>
					<Menu theme="dark" defaultSelectedKeys={['1']} mode="inline">
						<Menu.Item key="1">
							<Icon type="hdd" />
							<span>Your groups</span>
						</Menu.Item>
						{/* <Menu.Item key="2">
							<Icon type="sync" />
							<span>Refresh</span>
						</Menu.Item> */}
						<Menu.Item key="3" onClick={(e) => this.logout()}>
							<Icon type="logout" />
							<span>Logout</span>
						</Menu.Item>
					</Menu>
				</Sider>
				<Layout>
					<MenuBar />
					<Content className="content">
						{this.props.children}
					</Content>
				</Layout>
			</Layout >

		);
	}
	private renderEmty() {
		return (
			<Layout className="layout">
				<Row type='flex' align='middle' justify='space-around' >
					<Col span={8} >
						{this.props.children}
					</Col>
				</Row>
			</Layout>
		);
	}

	private logout() {
		if (this.props.isLogin) {
			firebase.auth.doSignOut().then(() => {
				localStorage.removeItem(constants.AUTH_IN_LOCAL_STORAGE);
				localStorage.removeItem(constants.ACCESS_TOKEN);
				this.props.logout();
			});
		}
	}
}

export default withRouter(connect(
	(state: ApplicationState) => ({
		...state.user
	}), // Selects which state properties are merged into the component's props
	dispatch => bindActionCreators({
		...userActionCreators
	}, dispatch) // Selects which action creators are merged into the component's props
)(PageLayout as any));
