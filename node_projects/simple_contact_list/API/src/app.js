const express = require("express");
const bodyParser = require("body-parser")
const userRoutes = require("./routes/userRoutes");
const contactRoutes = require("./routes/contactRoutes");
const mongoose = require("mongoose");
const cors = require("cors");
require("dotenv").config();

// MongoDB init
mongoose.connect(process.env.DB, { useNewUrlParser: true })
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
    res.header("Access-Control-Allow-Methods", "PUT, GET, POST, DELETE, DEL")
    next();
});

app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: true }));

// routes
app.use("/", userRoutes);
app.use("/contacts", contactRoutes);


app.use((err, req, res, next) => {
    console.log(err);
    next();
})

app.listen(port, ip_address, () => {
    console.log(`API running on ${ip_address}:${port}`);
});