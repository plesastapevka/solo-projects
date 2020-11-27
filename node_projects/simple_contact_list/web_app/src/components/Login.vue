<template>
      <v-container
        fluid
        class="fill-height"
      >
      <v-row align="center" justify="center">
          <v-col
            cols="1"
            sm="8"
            md="4"
          >
            <v-card class="elevation-12">
              <v-toolbar dark flat>
                <v-toolbar-title>Login</v-toolbar-title>
                <v-spacer></v-spacer>
              </v-toolbar>
              <v-card-text>
                <v-form>
                  <v-text-field
                    ref="email"
                    label="E-mail"
                    name="email"
                    prepend-icon="mdi-account"
                    type="text"
                    v-model="email"
                  >{{ email }}</v-text-field>

                  <v-text-field
                    ref="password"
                    label="Password"
                    name="password"
                    prepend-icon="mdi-lock"
                    type="password"
                    v-model="password"
                  >{{ password }}</v-text-field>
                  <p v-show="invalidLogin" align="center">Invalid credentials!</p>
                </v-form>
              </v-card-text>
              <v-layout column align-center justify-center class="fab-container pd-2">
                <v-btn fab>
                  <v-btn @click="login" fab color="primary"><v-icon>send</v-icon></v-btn>
                </v-btn>
                <div class="pd-2">
                  <v-btn to="/register" text small color="primary">Don't have an account?</v-btn>
                </div>
              </v-layout>
            </v-card>
          </v-col>
      </v-row>
      </v-container>
</template>

<script>
import api from '../services/services';
import router from '../router/index';
// import { EventBus } from '../router/index'

export default {
  name: "login",
  data() {
    return {
      email: null,
      password: null,
      error: null,
      invalidLogin: false
    }
  },

  methods: {
    async login() {
      let body = {
        mail: this.email,
        password: this.password
      }
      
      router.app.$session.destroy();

      api.postLogin(body, (err, res) => {
        if(err) {
          this.invalidLogin = true;
        } else {
          router.app.$session.start();
          router.app.$session.set('jwt', res.data.accessToken);
          router.app.$session.set('userId', res.data.id);
          router.app.$session.set('alive', true);
          // EventBus.$emit('loggedIn', true)
          this.$router.push('/');
        }
      });
    }
  }
}
</script>