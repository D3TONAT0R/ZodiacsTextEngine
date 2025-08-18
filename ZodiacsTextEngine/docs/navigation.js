document.addEventListener("DOMContentLoaded", function() {
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
                // Insert section links right after the active page link
                document.querySelectorAll("h2[id]").forEach(h2 => {
                    const sectionA = document.createElement("a");
                    sectionA.href = "#" + h2.id;
                    sectionA.textContent = h2.textContent;
                    sectionA.className = "section-link";
                    nav.appendChild(sectionA);
                });
                // Also include divs with class="group" and id
                document.querySelectorAll("div.group[id]").forEach(div => {
                    const groupA = document.createElement("a");
                    groupA.href = "#" + div.id;
                    // Use the first h2 inside the div for link text, or the div id if none
                    const firstH2 = div.querySelector("h2");
                    groupA.textContent = firstH2 ? firstH2.textContent.trim() : div.id;
                    groupA.className = "section-link";
                    nav.appendChild(groupA);
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