var express = require("express");
var router = express.Router();
var axios = require("axios").create({
  baseURL: "http://localhost:3333",
});

// GET home page
router.get("/", (req, res, next) => {
  if (req.session.userId) {
    axios
      .get("/room")
      .then((resp) => {
        res.render("index", {
          title: "Can you GUESS IT?",
          rooms: resp.data,
          session: req.session,
        });
      })
      .catch((err) => {
        res.render("error", {
          message: "Internal error",
          error: { status: 500 },
        });
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
    res.render("login", {});
  }
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

router.post("/logout", (req, res, next) => {
  req.session.destroy();
  res.redirect("/");
});

module.exports = router;
