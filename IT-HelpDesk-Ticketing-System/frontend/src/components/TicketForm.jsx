import {
    useEffect,
    useState
} from "react";

import {
    
    getCategories,
    getPriorities
} from "../services/lookupService";

function TicketForm({
    initialValues,
    onSubmit
}) {
    const [title, setTitle] =
        useState("");

    const [description, setDescription] =
        useState("");

    const [categoryId, setCategoryId] =
        useState("");

    const [priorityId, setPriorityId] =
        useState("");

    const [categories, setCategories] =
        useState([]);

    const [priorities, setPriorities] =
        useState([]);

    const [submitting, setSubmitting] =
        useState(false);

    useEffect(() => {
        loadLookups();
    }, []);

    useEffect(() => {
        if (!initialValues) {
            return;
        }

        setTitle(
            initialValues.title || ""
        );

        setDescription(
            initialValues.description || ""
        );

        setCategoryId(
            initialValues.categoryId || ""
        );

        setPriorityId(
            initialValues.priorityId || ""
        );
    }, [initialValues]);

    async function loadLookups() {
        try {
            const [
                categoriesData,
                prioritiesData
            ] = await Promise.all([
                getCategories(),
                getPriorities()
            ]);

            setCategories(categoriesData);
            setPriorities(prioritiesData);
        } catch (error) {
            console.error(
                "Lookup error:",
                error
            );

            alert(
                "Could not load categories or priorities."
            );
        }
    }

    async function submit(event) {
        event.preventDefault();

        if (!title.trim()) {
            alert("Title is required.");
            return;
        }

        if (!description.trim()) {
            alert("Description is required.");
            return;
        }

        if (!categoryId) {
            alert("Choose a category.");
            return;
        }

        if (!priorityId) {
            alert("Choose a priority.");
            return;
        }

        try {
            setSubmitting(true);

            await onSubmit({
                title: title.trim(),

                description:
                    description.trim(),

                categoryId:
                    Number(categoryId),

                priorityId:
                    Number(priorityId)
            });
        } finally {
            setSubmitting(false);
        }
    }

    return (
        <form
            onSubmit={submit}
            className="page-card"
        >
            <div className="form-grid">
                <div className="form-group">
                    <label htmlFor="title">
                        Title
                    </label>

                    <input
                        id="title"
                        type="text"
                        value={title}
                        onChange={(event) =>
                            setTitle(
                                event.target.value
                            )
                        }
                        required
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="category">
                        Category
                    </label>

                    <select
                        id="category"
                        value={categoryId}
                        onChange={(event) =>
                            setCategoryId(
                                event.target.value
                            )
                        }
                        required
                    >
                        <option value="">
                            Choose Category
                        </option>

                        {categories.map(
                            category => (
                                <option
                                    key={category.id}
                                    value={category.id}
                                >
                                    {category.name}
                                </option>
                            )
                        )}
                    </select>
                </div>

                <div className="form-group">
                    <label htmlFor="priority">
                        Priority
                    </label>

                    <select
                        id="priority"
                        value={priorityId}
                        onChange={(event) =>
                            setPriorityId(
                                event.target.value
                            )
                        }
                        required
                    >
                        <option value="">
                            Choose Priority
                        </option>

                        {priorities.map(
                            priority => (
                                <option
                                    key={priority.id}
                                    value={priority.id}
                                >
                                    {priority.name}
                                </option>
                            )
                        )}
                    </select>
                </div>

                <div className="form-group full-width">
                    <label htmlFor="description">
                        Description
                    </label>

                    <textarea
                        id="description"
                        value={description}
                        onChange={(event) =>
                            setDescription(
                                event.target.value
                            )
                        }
                        required
                    />
                </div>
            </div>

            <button
                type="submit"
                className="save-btn"
                disabled={submitting}
            >
                {submitting
                    ? "Saving..."
                    : "Save Ticket"}
            </button>
        </form>
    );
}

export default TicketForm;