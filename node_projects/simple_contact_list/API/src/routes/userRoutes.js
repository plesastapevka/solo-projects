const express = require("express");
const router = express.Router();
const UserModel = require("../models/userModel");
const bcrypt = require("bcrypt");
const saltRounds = 10;
const jwt = require("jsonwebtoken");
require("dotenv").config();

router.post("/register", async (req, res) => {
    let user = await UserModel.findOne({ mail: req.body.mail });
    if(user) return res.status(400).send({ message: "This user already exists." });
    
    let rawPassword = req.body.password;
    await bcrypt.genSalt(saltRounds, (err, salt) => {
        if(err) return res.status(500).send(err);
        bcrypt.hash(rawPassword, salt, (err, hash) => {
            if(err) return res.status(500).send(err);
            let password = hash;
            user = {
                name: req.body.name,
                lastName: req.body.lastName,
                username: req.body.username,
                mail: req.body.mail,
                password: password
            };
            UserModel.create(user, (err, newUser) => {
                if(err) return res.status(500).send(err);
                return res.status(200).send(newUser);
            });
        });
    });
});

router.post("/login", async (req, res) => {
    let password = req.body.password;
    let email = req.body.mail;
    await UserModel.findOne({ mail: email }, (err, user) => {
        if (err) return res.status(400).send({ status: 400, message: "Invalid username or password." });
        if (!user) return res.status(400).send({ status: 400, message: "Invalid username or password." });
        let rawPassword = password;
        let hashedPassword = user.password;
        bcrypt.compare(rawPassword, hashedPassword, (err, result) => {
            if (!result) return res.status(400).send({ status: 400, message: "Invalid username or password." });

            const token = jwt.sign({ id: user.id }, process.env.PRIVATE_KEY, { expiresIn: 86400 });
            return res.status(200).send({
                status: 200,
                id: user._id,
                username: user.username,
                mail: user.mail,
                accessToken: token
            });
        });
    });
});

module.exports = router;