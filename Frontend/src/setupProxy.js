const { createProxyMiddleware } = require('http-proxy-middleware');

module.exports = function(app) {
    app.use(
        '/api',
        createProxyMiddleware({
            target: 'https://localhost:5000/api',
            changeOrigin: true,
            secure: false,
            headers: {
                Connection: 'Keep-Alive'
            }
        })
    );
};