/**
 * @author [MrLong]
 * @description [websokcet 控制]
 */
import {getSetting} from '../common/commonHelper'
import pako from 'pako'
import reciveMsg from './modules/reciveMsg'

export default {
	state: {
		wsSocket: null,//websocket 连接对象
		url: null,//websocket 连接地址'192.168.1.106:1179'
		timeOut: 10000,
		timeOutObj: null,//心跳循环
		heart: 0,//心跳计数器
		reCon: 0,//重连计数器
		ReObj: null,//重连循环
		isSocket: false,//socket是否连接成功
		lockReconnect: false,//是否是首次连接
		isZ: "N",//通讯报文是否压缩 N--不压缩 Y --压缩
	},//state The End
	mutations: {			
		/**
		 * [upWsSocket 更新wsSocket的状态]
		 * @param  {[type]} state [description]
		 * @param  {[type]} evt   [description]
		 * @return {[type]}       [description]
		 */
		upWsSocket(state,evt) {
			state.wsSocket = evt
		},
		/**
		 * [upUrl 更新url的state]
		 * @param  {[type]} state [description]
		 * @param  {[type]} evt   [description]
		 * @return {[type]}       [description]
		 */
		upUrl(state,evt) {
			state.url = evt
		},
		/**
		 * [upHeart 更新heart]
		 * @param  {[type]} state [description]
		 * @param  {[type]} evt   [description]
		 * @return {[type]}       [description]
		 */
		upHeart(state,evt) {
			state.heart = evt
		},
		/**
		 * [upReCon 更新重连次数]
		 * @param  {[type]} state [description]
		 * @param  {[type]} evt   [description]
		 * @return {[type]}       [description]
		 */
		upReCon(state,evt) {
			state.reCon = evt
		},
		/**
		 * [upIsSocket 更新isSocket]
		 * @param  {[type]} state [description]
		 * @param  {[type]} evt   [description]
		 * @return {[type]}       [description]
		 */
		upIsSocket(state,evt) {
			state.isSocket = evt
		},
		/**
		 * [upLockReconnect 更新是否是首次连接]
		 * @param  {[type]} state [description]
		 * @param  {[type]} evt   [description]
		 * @return {[type]}       [description]
		 */
		upLockReconnect(state,evt) {
			state.lockReconnect = evt
		},
		/**
		 * [upIsZ 更新报文是否压缩]
		 * @param  {[type]} state [description]
		 * @param  {[type]} evt   [description]
		 * @return {[type]}       [description]
		 */
		upIsZ(state,evt) {
			state.isZ = evt
		}
	},//mutations The End
	actions: {
		/**
		 * [getConnectUrl 获取websocket连接地址]
		 * @return {[type]} [description]
		 */
		getConnectUrl({state,commit}) {
			getSetting((val)=>{
				var jsonData = JSON.parse(val);
				commit('upUrl',jsonData.socket);
			})
		},	
		/**
		 * [connetWebSocket 连接websocket]
		 * @param  {[type]} state [description]
		 * @return {[type]}       [description]
		 */
		connetWebSocket({commit,state,dispatch}) {	
			dispatch('getConnectUrl').then(setTimeout(()=>{
				//var store = {commit,state,dispatch};
				if(typeof(WebSocket) != undefined) {
					try {
						// console.log(connctUrl);
						commit('upWsSocket',new  WebSocket('ws://'+state.url+'/ws'));
						//state.wsSocket = new  WebSocket('ws://'+state.url+'/ws');		
					}catch (e) {
						console.log("网络连接异常,自动重连中...");
						state.wsSocket.close();
					}
				}else {
					console.log("您的浏览器不支持websocket");
					return;
				}
				//if(!state.wsSocket) return console.log("网络连接异常,请联系管理员...");
				/**
				 * [ononpen 连接成功打开]
				 * @return {[type]} [description]
				 */
				state.wsSocket.onopen = function() {
					commit('upLockReconnect',false);
					console.log('socket opened');
					dispatch('start');
					commit('upReCon',0);
					//state.reCon = 0;
					clearInterval(state.ReObj);
					commit('upIsSocket',true);
					commit('upIsZ',"Y");
					dispatch('sendSocketMsg',JSON.stringify({Head:"{\"Method\":\"HeartBeat\"}"}));//连接成功就发送心跳包
				}
				/**
				 * [onerror websocket连接出错]
				 * @return {[type]} [description]
				 */
				state.wsSocket.onerror = function() {
					console.log('socket error');
					state.wsSocket.close();
					commit('upIsSocket',false);
					// console.log(store);
					//state.isSocket = false;
				}
				/**
				 * [onclose 连接关闭]
				 * @return {[type]} [description]
				 */
				state.wsSocket.onclose = function() {
					console.log('socket closed');
					dispatch('reset');
					commit('upIsSocket',false);
					dispatch('reConnect');
				}	
				/**
				 * [onmessage 接收socket消息]
				 * @return {[type]} [description]
				 */
				state.wsSocket.onmessage = function(e) {
					// console.log(e);
					console.log('recive message');
					if(typeof(e.data) !== 'string' ) return;
					try {
						var reciveData = state.isZ == 'Y' ? pako.inflate(atob(e.data),{ to: 'string' }) : e.data;
						if(reciveData.indexOf('{') === -1) {
							console.log(reciveData);
						}else {
							var reciveJson = JSON.parse(reciveData);
							console.log(reciveData);
							dispatch('analyzeData',reciveJson);
						}		
					} catch(ex) {
						console.log(e.data);
					}
					
				}
			}),5)
				
		},//connetWebSocket The End
		/**
		 * [reset 重置心跳]
		 * @return {[type]} [description]
		 */
		reset(state) {
			clearInterval(state.timeOutObj);
		},//reset The End {commit,state,dispatch}
		/**
		 * [start 间隔timeOut毫秒向服务端发送心跳包]
		 * @return {[type]} [description]
		 */
		start({commit,state,dispatch}) {
			state.timeOutObj = setInterval(function() {
				if(!state.isSocket) return;
				commit('upIsZ',"Y");
				//这里发送一个心跳，后端收到后返回一个心跳消息，当再次收到心跳消息时则证明连接正常
				dispatch('sendSocketMsg',JSON.stringify({Head:"{\"Method\":\"HeartBeat\"}"}));
				commit('upHeart',(state.heart+1));
				//state.heart += 1;//心跳次数
				if(state.heart >= 3) {
					commit('upHeart',0);
					state.wsSocket.close();
				}
			},state.timeOut)
		},//start The End
		reConnect({commit,state,dispatch}) {
			if(state.lockReconnect) return;
			commit('upLockReconnect',true);
			state.ReObj = setInterval(function() {
				dispatch('connetWebSocket');
				commit('upReCon',(state.reCon+1));
				if(state.reCon >= 3) {
					clearInterval(state.ReObj);
					commit('upReCon',0);
					console.log("网络重连失败，请刷新重试");
				}
			},state.timeOut);
		},
		/**
		 * [sendSocketMsg 把报文进行gzip压缩后发送到服务端]
		 * @param  {[type]} msg [需要发送的报文]
		 * @return {[type]}     [description]
		 */
		sendSocketMsg({state},msg) {
			try {
				var binaryString = btoa(pako.gzip(msg,{ to: 'string' }));
				if(state.wsSocket && state.wsSocket.readyState === 1) {
					state.wsSocket.send(binaryString);
				} else {
					state.wsSocket.close();
					console.log("消息发送失败，请重试");
				}
			} catch(e) {
				console.log(e.message);
			}
		},//sengSocketMsg The End
	},//actions The End 
	modules: {
		reciveMsg,
	},//modules The End
}
