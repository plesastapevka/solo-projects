const bodyParser = require("body-parser")
const userRoutes = require("./routes/userRoutes");
const roomRoutes = require("./routes/roomRoutes");
const mongoose = require("mongoose");
const cors = require("cors");
const app = require("express")(cors());
const http = require("http").createServer(app);
const io = require("socket.io")(http, {
    cors: {
        origin: "*"
    }
});

require("./socket/socketDriver")(io);
require("dotenv").config();

// MongoDB init
mongoose.connect(process.env.DB, { useNewUrlParser: true })
    .then(() => console.log("Database connected"))
    .catch(err => console.log(err));

mongoose.Promise = global.Promise;


// Main REST API init
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
app.use("/room", roomRoutes);


app.use((err, req, res, next) => {
    console.log(err);
    next();
})

// io.on("connection", (socket) => {
//     console.log("Connected");
// });

http.listen(port, ip_address, () => {
    console.log(`API running on ${ip_address}:${port}`);
});