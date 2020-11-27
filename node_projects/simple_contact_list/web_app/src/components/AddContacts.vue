<template>
  <v-form class="pa-2" ref="form" v-model="valid" :lazy-validation="lazy">
      <v-text-field
        v-model="name"
        :counter="30"
        :rules="nameRules"
        label="Name*"
        required
      ></v-text-field>

      <v-text-field
        v-model="lastName"
        :counter="30"
        :rules="lastNameRules"
        label="Last name*"
        required
      ></v-text-field>

      <v-text-field
        v-model="number"
        :rules="numberRules"
        label="Phone number"
      ></v-text-field>

      <v-text-field
        v-model="email"
        :rules="emailRules"
        label="E-mail"
      ></v-text-field>


      <v-btn
        color="success"
        class="mr-4"
        @click="addContact"
      >
        CONFIRM
      </v-btn>
      <p v-show="error" align="center">{{ errorMsg }}</p>
    </v-form>
</template>
<script>
import api from '../services/services';
import router from '../router/index';

export default {
  name: "add_contacts",

  data() {
    return {
      name: '',
      lastName: '',
      number: '',
      email: '',
      emailRules: [ 
        v => !v || /^\w+([.-]?\w+)*@\w+([.-]?\w+)*(\.\w{2,3})+$/.test(v) || 'E-mail must be valid'
      ],
      error: false,
      errorMsg: ''
    }
  },

  methods: {
    addContact() {
      let body = {
        name: this.name,
        lastName: this.lastName,
        phoneNumber: this.number,
        email: this.email,
        userId: router.app.$session.get('userId'),
        accessToken: router.app.$session.get('jwt')
      }

      api.putAddContact(body, (err, res) => {
        if(err) {
          this.error = true;
          this.errorMsg = 'There has been an error!';
        } else {
          this.error = true;
          this.errorMsg = 'Contact added!';
        }
        return res;
      })
    }
  }
}
</script>

<style>

</style>