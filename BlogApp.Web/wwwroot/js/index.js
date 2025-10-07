let contextMenuExists = false;
let browserAuxMenuDisabled = true;
const main = document.querySelector("main");

main.addEventListener("mousedown", () => {
    if (contextMenuExists) {
        document.querySelectorAll(".context-menu").forEach(c => c.remove());
        document.querySelectorAll(".w98-item-toggled").forEach(i => i.classList.remove("w98-item-toggled"));
        contextMenuExists = false;
    }
});

main.addEventListener("auxclick", (e) => {
    const contextMenu = createContextMenu();

    const createPageItem = document.createElement("a");
    createPageItem.href = `/posts/create`;
    createPageItem.classList.add("context-menu-item");
    createPageItem.textContent = "Create New Blog Post";

    contextMenu.appendChild(createPageItem);
    contextMenu.appendChild(createBypassAuxMenuHintItem());

    const clientX = e.clientX;
    const clientY = e.clientY;
    contextMenu.style.top = `${clientY}px`;
    contextMenu.style.left = `${clientX}px`;

    document.body.appendChild(contextMenu);
    contextMenuExists = true;
});

const items = document.querySelectorAll(".grid-item");

items.forEach(item => {
    item.addEventListener("auxclick", (e) => {
        e.stopPropagation();
        item.classList.add("w98-item-toggled");

        const contextMenu = createContextMenu();
        const deletePageItem = document.createElement("a");
        deletePageItem.href = `/posts/${item.getAttribute("data-slug")}/delete`;
        deletePageItem.classList.add("context-menu-item");
        deletePageItem.textContent = "Delete Blog Post";

        contextMenu.appendChild(deletePageItem);
        contextMenu.appendChild(createBypassAuxMenuHintItem());

        const clientX = e.clientX;
        const clientY = e.clientY;
        contextMenu.style.top = `${clientY}px`;
        contextMenu.style.left = `${clientX}px`;

        document.body.appendChild(contextMenu);
        contextMenuExists = true;
    });
});

window.addEventListener('contextmenu', (e) => {
    if (!e.shiftKey)
        e.preventDefault();
});

const createContextMenu = () => {
    const contextMenu = document.createElement("div");
    contextMenu.classList.add("context-menu");
    contextMenu.classList.add("w98-border");
    return contextMenu;
}

const createBypassAuxMenuHintItem = () => {
    const item = document.createElement("div");
    item.textContent = "Shift+rclick to bypass";
    item.style.padding = "0px 1.5rem 2px";
    item.style.color = "rgb(112, 111, 111)";
    item.style.fontStyle = "italic";
    return item;
}