document.addEventListener("DOMContentLoaded", () => {


    const modal = new bootstrap.Modal(document.getElementById("taskModal"));
    const modalContent = document.getElementById("taskModalContent");
    /*240226
    // Nueva función para cargar categorías 190226
    async function fillCategoriesSelect(selectedId) {
        const select = document.getElementById("CategoryIdSelect");
        const response = await fetch("/Categories/GetCategoriesJson");
        const categories = await response.json();

        select.innerHTML = '<option value="">-- Sin Categoria --</option>';
        categories.forEach(cat => {
            const isSelected = cat.id == selectedId ? "selected" : "";
            select.innerHTML += `<option value="${cat.id}" ${isSelected}>${cat.name}</option>`;
        });
    }
    */
    // CREAR
    document.getElementById("btnCrearTask")
        .addEventListener("click", async () => {

            modalContent.innerHTML = spinnerHtml();

            const response = await fetch("/Tasks/CreatePartial");
            console.log(response);
            const html = await response.text();

            modalContent.innerHTML = html;
            // 🔹 Cargar categorías después de insertar el HTML 240226
            await loadCategoriesInModal(modalContent);
            modal.show();
        });

    // EDITAR
    document.addEventListener("click", async (e) => {
        if (e.target.matches(".btnEdit")) {

            const id = e.target.dataset.id;

            modalContent.innerHTML = spinnerHtml();

            const response = await fetch('/Tasks/EditPartial/' + id);
            const html = await response.text();

            modalContent.innerHTML = html;
            console.log("Hola");
            // 🔹 Cargar categorías después de insertar el HTML
            await loadCategoriesInModal(modalContent);
            modal.show();

          //  const categoryId = document.getElementById("currentCategoryId").value;
           // await fillCategoriesSelect(categoryId);
        }
    });

    // GUARDAR
    document.addEventListener("click", async (e) => {
        if (e.target.id === "btnSaveTask") {

            const form = document.getElementById("taskForm");
            const formData = new FormData(form);

            const data =
                {
                Id: parseInt(formData.get("Id")) || 0,
                Title: formData.get("Title"),
                CategoryId: parseInt(formData.get("CategoryId")) || 0,
                Step: parseInt(formData.get("Step")) || 0,
                // Verifica si es true (uso futuro)?) o viene del input oculto
                //IsCompleted: formData.get("IsCompleted") === "true"
                IsCompleted: form.querySelector('input[name="IsCompleted"][type="checkbox"]').checked
                }

            const isEdit = data.Id && data.Id !== "0";

            const url = isEdit
                ? '/Tasks/Edit/' + data.Id
                    : '/Tasks/Create';

            const response = await fetch(url,
                {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(data)
                });

            if (!response.ok)
                {
                const html = await response.text();
                modalContent.innerHTML = html;
                return;
                }

            modal.hide();

            await refreshTable();
        }
    });
    //ELIMINAR 260226 
    document.addEventListener("click", async (e) => {
        if (e.target.matches(".btnDelete")) {

            const id = e.target.dataset.id;
            if (!id) return;

            const confirmado = confirm("¿Seguro que deseas eliminar esta tarea?");
            if (!confirmado) return;

            try {
                const response = await fetch('/Tasks/DeleteAjax/' + id, {
                    method: "POST"
                });

                const result = await response.json();

                if (!response.ok || !result.success) {
                    showError(result.message || "No se pudo eliminar la tarea.");
                    return;
                }

                showSuccess(result.message || "La tarea fue eliminada correctamente.");

                await refreshTable();

            } catch (err) {
                console.error(err);
                showError("Error de comunicación con el servidor al eliminar la tarea.");
            }
        }
    });



});
// Función spinnerHtml
function spinnerHtml() {
    return `
        <div class="modal-body text-center">
            <div class="spinner-border text-primary"></div>
            <p>Cargando...</p>
        </div>`;
}

// Función refreshTable
async function refreshTable() {
    const response = await fetch("/Tasks/LoadTablePartial");
    const html = await response.text();
    document.getElementById("taskTableContainer").innerHTML = html;
}

//240226
// Función para cargar las categorías en el select del modal 240226
async function loadCategoriesInModal(modalContent) {
    console.log("Hola");
    const select = modalContent.querySelector("#categorySelect");
    if (!select) return;

    // Valor actual (cuando edito)
    const selectedId = select.dataset.selectedCategoryId || "";
    console.log(select.dataset);
    try {
        const response = await fetch("/Categories/Options");

        if (!response.ok) {
            console.error("Error al cargar categorías");
            return;
        }

        const categories = await response.json();

        // Limpiamos las opciones actuales excepto la primera
        const firstOption = select.querySelector("option[value='']");
        select.innerHTML = "";
        if (firstOption) {
            select.appendChild(firstOption);
        } else {
            const defaultOpt = document.createElement("option");
            defaultOpt.value = "";
            defaultOpt.textContent = "-- Seleccione una categoría --";
            select.appendChild(defaultOpt);
        }

        // Agregamos las categorías
        categories.forEach(cat => {
            const opt = document.createElement("option");
            opt.value = cat.id;
            opt.textContent = cat.name;

            if (selectedId && selectedId === String(cat.id)) {
                opt.selected = true;
            }

            select.appendChild(opt);
        });

    } catch (err) {
        console.error("Error de red al cargar categorías", err);
    }
}

// ✅ Funciones auxiliares para mostrar mensajes 260226
function showSuccess(message) {
    const container = document.getElementById("alertContainer");
    if (!container) return;

    container.innerHTML = `
       <div class="alert alert-success alert-dismissible fade show" role="alert">
           ${message}
           <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
       </div>`;
}

function showError(message) {
    const container = document.getElementById("alertContainer");
    if (!container) return;

    container.innerHTML = `
       <div class="alert alert-danger alert-dismissible fade show" role="alert">
           ${message}
           <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
       </div>`;
}