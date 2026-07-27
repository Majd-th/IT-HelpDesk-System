import { useState, useEffect } from "react";
import {
    getCategories,
    getPriorities,
    getStatuses
} from "../services/lookupService";

function TicketForm({
    initialValues,
    onSubmit
}) {

    const [title, setTitle] = useState("");
    const [description, setDescription] = useState("");
    const [categoryId, setCategoryId] = useState("");
    const [priorityId, setPriorityId] = useState("");
    const [statusId, setStatusId] = useState("");
    const [solution, setSolution] = useState("");

    const [categories, setCategories] = useState([]);
    const [priorities, setPriorities] = useState([]);
    const [statuses, setStatuses] = useState([]);

    useEffect(() => {
        loadLookups();
    }, []);

    useEffect(() => {

        if (!initialValues) return;

        setTitle(initialValues.title || "");
        setDescription(initialValues.description || "");
        setCategoryId(initialValues.categoryId || "");
        setPriorityId(initialValues.priorityId || "");
        setStatusId(initialValues.statusId || "");
        setSolution(initialValues.solution || "");

    }, [initialValues]);

    async function loadLookups() {
        try {
            const categoriesData = await getCategories();
            const prioritiesData = await getPriorities();
            const statusesData = await getStatuses();

            setCategories(categoriesData);
            setPriorities(prioritiesData);
            setStatuses(statusesData);

            console.log(categoriesData);
            console.log(prioritiesData);
            console.log(statusesData);

        } catch (err) {
            console.error("Lookup error:", err);
        }
    }

    function submit(e) {

        e.preventDefault();

        onSubmit({
            title,
            description,
            categoryId: Number(categoryId),
            priorityId: Number(priorityId),
            statusId: Number(statusId),
            solution
        });
    }

    return (
        <form onSubmit={submit} className="page-card">

            <div className="form-grid">

                <div className="form-group">
                    <label>Title</label>

                    <input
                        value={title}
                        onChange={(e) => setTitle(e.target.value)}
                    />
                </div>

                <div className="form-group">
                    <label>Category</label>

                    <select
                        value={categoryId}
                        onChange={(e) => setCategoryId(e.target.value)}
                    >
                        <option value="">
                            Choose Category
                        </option>

                        {categories.map(c => (
                            <option key={c.id} value={c.id}>
                                {c.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="form-group">
                    <label>Priority</label>

                    <select
                        value={priorityId}
                        onChange={(e) => setPriorityId(e.target.value)}
                    >
                        <option value="">
                            Choose Priority
                        </option>

                        {priorities.map(p => (
                            <option key={p.id} value={p.id}>
                                {p.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="form-group">
                    <label>Status</label>

                    <select
                        value={statusId}
                        onChange={(e) => setStatusId(e.target.value)}
                    >
                        <option value="">
                            Choose Status
                        </option>

                        {statuses.map(s => (
                            <option key={s.id} value={s.id}>
                                {s.name}
                            </option>
                        ))}
                    </select>
                </div>

                <div className="form-group full-width">
                    <label>Description</label>

                    <textarea
                        value={description}
                        onChange={(e) => setDescription(e.target.value)}
                    />
                </div>

                <div className="form-group full-width">
                    <label>Solution</label>

                    <textarea
                        value={solution}
                        onChange={(e) => setSolution(e.target.value)}
                    />
                </div>

            </div>

            <button className="save-btn">
                Save Ticket
            </button>

        </form>
    );
}

export default TicketForm;