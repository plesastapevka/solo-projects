var express = require("express");
var router = express.Router();
var axios = require("axios").create({
  baseURL: "http://localhost:3333",
});

// GET home page
router.get("/", (req, res, next) => {
  if (req.session.userId) {
    res.render("index", {
      title: "Can you GUESS IT?",
      session: req.session,
    });
  } else {
    res.redirect("/login");
  }
});

// GET login page
router.get("/login", (req, res, next) => {
  if (req.session.userId) {
    res.redirect("/");
  } else {
    res.render("login");
  }
});

// GET register page
router.get("/register", (req, res, next) => {
  if (req.session.userId) {
    res.redirect("/");
  } else {
    res.render("register");
  }
});

// GET leaderboards page
router.get("/leaderboards", (req, res, next) => {
  axios
    .get("/")
    .then((resp) => {
      let users = resp.data.users;
      users.sort((a, b) =>
        a.score < b.score
          ? 1
          : a.score === b.score
          ? a.globalWins > b.globalWins
            ? 1
            : -1
          : -1
      );
      res.render("leaderboards", { users: users });
    })
    .catch((err) => {
      res.render("error", {
        message: "Internal error",
        error: { status: 500 },
      });
    });
});

// POST login
router.post("/login", (req, res, next) => {
  let username = req.body.username;
  let password = req.body.password;
  const body = {
    username: username,
    password: password,
  };
  axios
    .post("/login", body)
    .then((resp) => {
      req.session.userId = resp.data.id;
      req.session.username = resp.data.username;
      req.session.bearer = resp.data.accessToken;
      res.redirect("/");
    })
    .catch((err) => {
      if (err.response.status == 400) {
        res.render("login", { message: "Invalid username or password." });
      } else {
        res.render("error", {
          message: "Internal error",
          error: { status: 500 },
        });
      }
    });
});

// POST register
router.post("/register", (req, res, next) => {
  let username = req.body.username;
  let password = req.body.password;
  let repeat = req.body.repeatPassword;
  if (
    !password ||
    !repeat ||
    password === "" ||
    repeat === "" ||
    password.includes(" ") ||
    repeat.includes(" ")
  ) {
    res.render("register", { message: "Password field cannot be empty" });
    return;
  }
  if (password !== repeat) {
    res.render("register", { message: "Passwords must match" });
    return;
  }
  const body = {
    username: username,
    password: password,
  };
  axios
    .post("register", body)
    .then((resp) => {
      res.redirect("/login");
    })
    .catch((err) => {
      res.render("register", { message: err.response.data.message });
    });
});

// POST logout
router.post("/logout", (req, res, next) => {
  req.session.destroy();
  res.redirect("/");
});

module.exports = router;
