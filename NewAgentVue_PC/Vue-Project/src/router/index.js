import Vue from 'vue'
import Router from 'vue-router'
import store from '../store'
import HelloWorld from '@/components/HelloWorld'
import login from '@/components/login'

Vue.use(Router)

const vueRouter =  new Router({
  routes: [
    {
      path: '/',
      name: 'login',
      component: login,
      meta:{requiresAuth: false}
    }
  ]
});

// 全局路由守卫
vueRouter.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuth)) {
    // this route requires auth, check if logged in
    // if not, redirect to login page.
    if (store.state.isLogin=='0') {
      next({
        path: '/login',
        query: { redirect: to.fullPath }
      })
    } else {
      next()
    }
  } else {
    next() // 确保一定要调用 next()
  }
});


export default  vueRouter;
