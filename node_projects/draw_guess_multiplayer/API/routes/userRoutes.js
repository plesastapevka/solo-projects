const express = require("express");
const router = express.Router();
const UserModel = require("../models/userModel");
const bcrypt = require("bcrypt");
const saltRounds = 10;
const jwt = require("jsonwebtoken");
require("dotenv").config();

router.post("/register", async (req, res) => {
    let user = await UserModel.findOne({ username: req.body.username });
    if(user) return res.status(400).send({ message: "This user already exists." });
    
    let rawPassword = req.body.password;
    await bcrypt.genSalt(saltRounds, (err, salt) => {
        if(err) return res.status(500).send(err);
        bcrypt.hash(rawPassword, salt, (err, hash) => {
            if(err) return res.status(500).send(err);
            let password = hash;
            user = {
                username: req.body.username,
                password: password,
                score: 0,
                globalWins: 0
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
    let username = req.body.username;
    await UserModel.findOne({ username: username }, (err, user) => {
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
                accessToken: token
            });
        });
    });
});

router.get("/", async (req, res) => {
    UserModel.find({}, (err, users) => {
        if (err) {
            console.log(err);
            return res.status(500).send({ status: 500, message: "Could not fetch users" });
        }
        return res.status(200).send({ status: 200, users: users });
    });
});

module.exports = router;