<template>
  <div id="app">
    <!-- <img src="./assets/logo.png"> -->
    <router-view/>
    <loading />
  </div>
</template>

<script>
import {mapActions} from 'vuex'
import loading from './common/animate.vue'

export default {
  name: 'App',
  methods: {
    ...mapActions([
        'connetWebSocket'
      ]),
    refresh (){
          // this.$confirm("页面刷新了");
          this.$router.push('/');
        },
  },//methods The End
  created: function() {
    this.connetWebSocket();//连接socket
    window.addEventListener("beforeunload", this.refresh());   
  },//created The End 
  components: {
    loading,
  },//components The End
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
