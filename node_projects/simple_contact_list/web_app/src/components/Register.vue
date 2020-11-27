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
                <v-toolbar-title>Registration</v-toolbar-title>
                <v-spacer></v-spacer>
              </v-toolbar>
              <v-card-text>
                <v-text-field
                    ref="name"
                    label="Name"
                    name="name"
                    type="text"
                    v-model="name"
                  >{{ name }}</v-text-field>
                <v-text-field
                    ref="lastName"
                    label="Last Name"
                    name="lastName"
                    type="text"
                    v-model="lastName"
                  >{{ lastName }}</v-text-field>
                <v-text-field
                    ref="username"
                    label="Username"
                    name="username"
                    type="text"
                    v-model="username"
                  >{{ lastName }}</v-text-field>
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
                  <p v-show="invalidRegistration" align="center">Invalid credentials!</p>
                </v-form>
              </v-card-text>
              <v-layout column align-center justify-center class="fab-container pd-2">
                <v-btn fab>
                  <v-btn @click="register" fab color="primary"><v-icon>send</v-icon></v-btn>
                </v-btn>
                <div class="pd-2">
                  <v-btn to="/login" text small color="primary">Already have an account?</v-btn>
                </div>
              </v-layout>
            </v-card>
          </v-col>
      </v-row>
      </v-container>
</template>

<script>
import api from '../services/services';
// import router from '../router/index';

export default {
  name: "register",
  data() {
    return {
      name: null,
      lastName: null,
      username: null,
      email: null,
      password: null,
      error: null,
      invalidRegistration: false
    }
  },

  methods: {
    registration() {
      console.log("REGISTRATION");
    },

    async register() {
      let body = {
        name: this.name,
        lastName: this.lastName,
        username: this.username,
        mail: this.email,
        password: this.password
      }

      api.postRegister(body, (err, res) => {
        if(err) {
          this.invalidRegistration = true;
        } else {
          this.$router.push('/login');
        }

        return res;
      });
    }
  }
}
</script>