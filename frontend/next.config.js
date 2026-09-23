/** @type {import('next').NextConfig} */
const nextConfig = {
  images: {
    remotePatterns: [
      {
        protocol: "http",
        hostname: "localhost",
      },
      {
        // Ajuste para o domínio real da API em produção
        protocol: "https",
        hostname: "**",
      },
    ],
  },
};

module.exports = nextConfig;
