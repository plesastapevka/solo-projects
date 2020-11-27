import API from './api'

export default {
    async getAllContacts(params) {
        return API().get('/contacts', params);
    },

    async postLogin(body, cb) {
        API()
        .post('/login', body)
        .then((res) => {
            cb(null, res);
            return res;
        })
        .catch((err) => {
            cb(err);
            return null;
        })
    },

    async postRegister(body, cb) {
        API()
        .post('/register', body)
        .then((res) => {
            cb(null, res);
            return res;
        })
        .catch((err) => {
            cb(err);
            return null;
        })
    },

    async putAddContact(params, cb) {
        API()
        .put('/contacts/0', params)
        .then((res) => {
            cb(null, res);
            return res;
        })
        .catch((err) => {
            cb(err, null);
            return null;
        })
    },

    async putUpdateContact(id, params, cb) {
        API()
        .put('/contacts/' + id, params)
        .then((res) => {
            cb(null, res);
            return res;
        })
        .catch((err) => {
            cb(err, null);
            return null;
        })
    },

    async postDeleteContact(body, headers, cb) {
        API()
        .post('/contacts', body, headers)
        .then((res) => {
            cb(null, res);
            return res;
        })
        .catch((err) => {
            cb(err, null);
            return null;
        })
    }
}