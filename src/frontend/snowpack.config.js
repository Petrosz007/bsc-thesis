// Snowpack Configuration File
// See all supported options: https://www.snowpack.dev/reference/configuration

/** @type {import("snowpack").SnowpackUserConfig } */
module.exports = {
  mount: {
    'src': '/dist',
    'public': { url: '/', static: true },
  },
  plugins: [
    '@snowpack/plugin-react-refresh',
    '@snowpack/plugin-sass',
    '@snowpack/plugin-typescript',
    ['@snowpack/plugin-webpack',
    {
      // sourceMap: true,
    }
    ]
  ],
  packageOptions: {
    // "source": "remote",
  },
  devOptions: {
    hostname: '127.0.0.1',
    port: 8100,
  },
  buildOptions: {
  },
  routes: [
    { match: "routes", src: ".*", dest: "/index.html" }
  ],
};
