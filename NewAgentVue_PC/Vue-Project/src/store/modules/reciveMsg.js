/**
 * @author [MrLong]
 * @description [接收服务端发送过来的消息]
 */
export default {
	state:{

	},
	mutations: {

	},
	actions: {
		/**
		 * [analyzeData 解析服务端发送过来的报文]
		 * @param  {[type]} msg [收到的报文]
		 * @return {[type]}     [description]
		 */
		analyzeData({dispatch,commit,state,rootSate},msg) {
			var headMsg = JSON.parse(msg.Head),
				error = JSON.parse(msg.Error);
			if(headMsg && headMsg.Method) {
				switch(headMsg.Method) {
					case "HeartBeat":
						commit('upHeart',0);//收到心跳包则重置心跳
						//dispatch('reset',null,{ root: true });
						break;		
				}
			}//状态码判断结束	

		}
	},
}