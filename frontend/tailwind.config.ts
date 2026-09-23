import type { Config } from "tailwindcss";

const config: Config = {
  content: ["./src/**/*.{js,ts,jsx,tsx,mdx}"],
  theme: {
    extend: {
      colors: {
        brand: {
          DEFAULT: "#C41230",
          dark: "#8F0E24",
          light: "#E94E64",
        },
        ink: "#10182B",
        navy: "#0F2A4A",
        surface: "#F4F4F5", // Um tom de cinza bem suave e moderno
        line: "#E4E4E7",
        success: "#157F4B",
        warning: "#B5530B",
        danger: "#B42318",
      },
      fontFamily: {
        sans: ["var(--font-inter)", "sans-serif"],
      },
      boxShadow: {
        card: "0 1px 3px 0 rgb(0 0 0 / 0.1), 0 1px 2px -1px rgb(0 0 0 / 0.1)",
        dropdown: "0 4px 6px -1px rgb(0 0 0 / 0.1), 0 2px 4px -2px rgb(0 0 0 / 0.1)",
      },
      borderRadius: {
        card: "12px",
      },
    },
  },
  plugins: [],
};

export default config;
