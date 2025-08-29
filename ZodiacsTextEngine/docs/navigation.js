document.addEventListener("DOMContentLoaded", function () {
    // List of documentation pages
    const pages = [
        { href: "index.html", label: "ZodiacsTextEngine", class: "title" },
        { href: "getting-started.html", label: "Getting Started" },
        { href: "room-files.html", label: "Room File Structure" },
        { href: "effects.html", label: "Effects" },
        { href: "rich-text.html", label: "Rich Text" },
        { href: "variables.html", label: "Variables" },
        { href: "custom-functions.html", label: "Custom Functions" },
        { href: "library-usage.html", label: "Using ZodiacsTextEngine in a custom Application" }
    ];

    // Get current page filename
    const currentPage = location.pathname.split("/").pop();

    // Create nav element
    const nav = document.createElement("nav");

    // Add page links
    let sectionLinksInserted = false;
    pages.forEach(page => {
        const a = document.createElement("a");
        a.href = page.href;
        a.textContent = page.label;
        if (page.class) a.className = page.class;
        if (page.href === currentPage) {
            a.classList.add("active");
            nav.appendChild(a);
            const elements = Array.from(document.querySelectorAll("h2[id], div.group[id], h3[id]"));
            elements.forEach(el => {
                let sectionA = document.createElement("a");
                sectionA.href = "#" + el.id;
                if (el.tagName.toLowerCase() === "h2") {
                    sectionA.textContent = el.textContent;
                    sectionA.className = "section-link";
                } else if (el.tagName.toLowerCase() === "h3") {
                    sectionA.textContent = el.textContent;
                    sectionA.className = "subsection-link";
                } else {
                    const firstH2 = el.querySelector("h2");
                    sectionA.textContent = firstH2 ? firstH2.textContent.trim() : el.id;
                    sectionA.className = "group-link";
                }
                nav.appendChild(sectionA);
            });
            sectionLinksInserted = true;
        } else {
            nav.appendChild(a);
        }
    });

    // Insert nav at the top of <body>
    const body = document.body;
    const oldNav = body.querySelector("nav");
    if (oldNav) oldNav.remove();
    body.insertBefore(nav, body.firstChild);
});