import * as React from 'react';
import { Container } from 'reactstrap';
import NavMenu from './NavMenu';
import { Layout, Menu, Breadcrumb, Icon, Divider, Row, Col } from 'antd';
import '../styles/Layout.css';
import { constants } from '../constants/constant';
import * as firebase from '../firebase';

const { Header, Sider, Content, Footer } = Layout;

class PageLayout extends React.Component<
	any,
	{
		collapsed: boolean;
	}
	> {
	state = {
		collapsed: false
	};

	onCollapse = (collapsed: boolean) => {
		this.setState({ collapsed });
	};

	render() {
		const authData = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
		return (<>{authData ? this.renderLayout() : this.renderEmty()}</>);
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
						<Menu.Item key="3" onClick={this.logout}>
							<Icon type="logout" />
							<span>Logout</span>
						</Menu.Item>
					</Menu>
				</Sider>
				<Layout>
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

	private logout(){
		const authData = localStorage.getItem(constants.AUTH_IN_LOCAL_STORAGE);
		if(authData != null){
			firebase.auth.doSignOut();
			localStorage.removeItem(constants.AUTH_IN_LOCAL_STORAGE);
			localStorage.removeItem(constants.ACCESS_TOKEN);
			window.location.href = "/";
		}
	}
}

export default PageLayout;
