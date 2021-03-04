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
    port: 8100,
  },
  buildOptions: {
  },
};
