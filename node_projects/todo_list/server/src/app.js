const express = require("express");
const bodyParser = require("body-parser")
const listRoutes = require("./routes/listRoutes");
const taskRoutes = require("./routes/taskRoutes")
const mongoose = require("mongoose");
const cors = require("cors");
require("dotenv").config();

// MongoDB init
mongoose.connect(process.env.DB, { useNewUrlParser: true, useUnifiedTopology: true })
    .then(() => console.log("Database connected"))
    .catch(err => console.log(err));

mongoose.Promise = global.Promise;


// Main REST API init
const app = express(cors());
const port = process.env.API_PORT;
const ip_address = process.env.IP_ADDR;

app.use((req, res, next) => {
    res.header("Access-Control-Allow-Origin", "*");
    res.header("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, x-access-token");
    res.header("Access-Control-Allow-Methods", "PUT, GET, POST, DELETE")
    next();
});

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// routes
app.use("/lists", listRoutes);
app.use("/tasks", taskRoutes);


app.use((err, req, res, next) => {
    console.log(err);
    next();
})

app.listen(port, ip_address, () => {
    console.log(`API running on ${ip_address}:${port}`);
});