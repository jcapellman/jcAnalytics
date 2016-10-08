var restify = require("restify");
var settings = require("./config");

var predictRouter = require("./routes/predict-router");

var server = restify.createServer();

server.use(restify.queryParser());

predictRouter.applyRoutes(server);

server.listen(settings.HTTP_SERVER_PORT);