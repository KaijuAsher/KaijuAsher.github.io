const http = require('http');

const hostname = '127.0.0.1';
const port = 8888;

const server = http.createServer((req, res) => {
  const options = {
    hostname: 'khanacademy.me',
    port: 80,
    path: req.url,
    method: req.method,
    headers: req.headers
  };

  const proxyReq = http.request(options, (proxyRes) => {
    proxyRes.pipe(res, {
      end: true
    });
  });

  req.pipe(proxyReq, {
    end: true
  });
});

server.listen(port, hostname, () => {
  console.log(`Proxy server running at http://${hostname}:${port}/`);
});
