/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        "./**/*.razor",
        "./**/*.html",
        "./**/*.cshtml",
        "!./**/bin/**",
        "!./**/obj/**",
    ],
    theme: {
        extend: {
            keyframes: {
                floatOrb: {
                    from: { transform: "translate3d(0,0,0) scale(1)" },
                    to: { transform: "translate3d(40px,-30px,0) scale(1.08)" },
                },
                modalFade: {
                    from: { opacity: "0" },
                    to: { opacity: "1" },
                },
                modalUp: {
                    from: { opacity: "0", transform: "translateY(20px) scale(.98)" },
                    to: { opacity: "1", transform: "translateY(0) scale(1)" },
                },
                spin: {
                    to: { transform: "rotate(360deg)" },
                },
            },
            animation: {
                "float-orb": "floatOrb 12s ease-in-out infinite alternate",
                "modal-fade": "modalFade .25s ease",
                "modal-up": "modalUp .3s ease",
                "spin": "spin .7s linear infinite",
            },
        },
    },
    plugins: [],
};