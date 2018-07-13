<!--
    作者：offline
    时间：2017-04-19
    描述：登陆页面
-->
<template>
<div id="window" style="display:none;">
	<div class="page login">
		<div class="page-content">
			<!-- 财神娱乐  港都国际  和盛娱乐-->
			<div class="loginName">{{this.title}}代理管理</div>
			<div class="input-row">
				<input id="txtUname" v-model="gname" type="text" placeholder="输入代理商名称" class="input fadeIn delay1" @keydown="toKeyDown" >
			</div>
			<div class="input-row">
				<input id="txtPWD" v-model="gpass" type="password" placeholder="输入代理商密码" class="input fadeIn delay3" @keydown="toKeyDown">
			</div>
			<div class="input-row perspective">
				<button id="submit" class="button load-btn fadeIn delay4" @click = "toHall">
					<span class="default"><i class="fa fa-unlock-alt"></i>登 录</span>
					<div class="load-state">
						<div class="ball"></div>
						<div class="ball"></div>
						<div class="ball"></div>
					</div>
				</button>
			</div>
			<div id="hint">{{hint}}</div>
		</div>
	</div>
</div>
</template>

<script>
import {getSetting} from '../common/commonHelper.js'
// 导入vuex
import { mapState, mapMutations, mapActions } from 'vuex'

export default {
	name:'index',
	data(){
		return{
			loginData:"",
			Timeout:null,
			title:null,
		}
	},
	components: {},
	computed:{
		/**
		 * @description [从store中引入需要的数据]
		 */
		...mapState({
			hint: state => state.loginStore.G_hint,
			_submit: state => state.loginStore.G_submit
		}),	
		gname: {
			get() {
				return  this.$store.state.loginStore.G_name;
			},
			set(value) {
				this.upName(value);
			}
		},
		gpass: {
			get() {
				return this.$store.state.loginStore.G_pass;
			},
			set(value) {
				this.upPass(value);
			}
		},

	},
	methods:{
		...mapActions([
			'login'
		]),
		...mapMutations([
			'upSubmit',
			'upPass',
			'upName',
			'upHint'
		]),
		toHall(){//登陆事件
			$("#hint").hide();
			console.log(this.gname);
			if(this.gname && this.gpass){	
				this.initAnimation();
				this.login();
			}else{
				$("#hint").show();
				this.upHint('请输入帐号和密码');
				this.upPass('');
				$('#txtPWD').focus();				
			}
		},		
		toKeyDown(){
			if(event.keyCode == 13 && !$('#submit').attr("disabled")){
				this.toHall();
			}
		},
		initAnimation(){
			$('#submit').addClass('loading');
			$('#submit').attr("disabled",true);
			if($("#submit").hasClass("done")) $("#submit").removeClass("done");
		},
		GetData(val){
		   var data = JSON.parse(val);
		   this.title = data.title;
		},
	},
	mounted(){
		$('#window').attr('style', ''); 
		$('#txtUname').focus();
		getSetting(this.GetData);
	},	
}
</script>

<style scoped >
@import '../style/login.css';
#hint{position: absolute;z-index: 9999999;color: red;top: 282px;}
</style>



