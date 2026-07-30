import { useEffect, useState } from "react";

import {
    getCategories,
    getPriorities,
    getStatuses
} from "../services/lookupService";

function TicketFilter({ onFilter, onReset }) {
    const [filters, setFilters] = useState({
        search: "",
        categoryId: "",
        priorityId: "",
        statusId: "",
        dateRange: "",
        createdAfter: "",
        createdBefore: ""
    });

    const [categories, setCategories] = useState([]);
    const [priorities, setPriorities] = useState([]);
    const [statuses, setStatuses] = useState([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        loadLookups();
    }, []);

    async function loadLookups() {
        try {
            setLoading(true);

            const [
                categoryData,
                priorityData,
                statusData
            ] = await Promise.all([
                getCategories(),
                getPriorities(),
                getStatuses()
            ]);

            setCategories(categoryData);
            setPriorities(priorityData);
            setStatuses(statusData);
        } catch (error) {
            console.error("Could not load ticket filters:", error);
        } finally {
            setLoading(false);
        }
    }

    function handleChange(event) {
        const { name, value } = event.target;

        setFilters((current) => ({
            ...current,
            [name]: value
        }));
    }

    function calculateDates(dateRange) {
        if (!dateRange) {
            return {
                createdAfter: "",
                createdBefore: ""
            };
        }

        const now = new Date();
        const after = new Date();

        if (dateRange === "day") {
            after.setDate(now.getDate() - 1);
        }

        if (dateRange === "week") {
            after.setDate(now.getDate() - 7);
        }

        if (dateRange === "month") {
            after.setMonth(now.getMonth() - 1);
        }

        if (dateRange === "year") {
            after.setFullYear(now.getFullYear() - 1);
        }

        return {
            createdAfter: after.toISOString(),
            createdBefore: now.toISOString()
        };
    }

    function handleSubmit(event) {
        event.preventDefault();

        const dates = calculateDates(filters.dateRange);

        onFilter({
            search: filters.search.trim(),
            categoryId: filters.categoryId || null,
            priorityId: filters.priorityId || null,
            statusId: filters.statusId || null,
            createdAfter:
                filters.createdAfter || dates.createdAfter || null,
            createdBefore:
                filters.createdBefore || dates.createdBefore || null
        });
    }

    function handleReset() {
        const emptyFilters = {
            search: "",
            categoryId: "",
            priorityId: "",
            statusId: "",
            dateRange: "",
            createdAfter: "",
            createdBefore: ""
        };

        setFilters(emptyFilters);

        if (onReset) {
            onReset();
        } else {
            onFilter({
                search: "",
                categoryId: null,
                priorityId: null,
                statusId: null,
                createdAfter: null,
                createdBefore: null
            });
        }
    }

    return (
       <form
    className="ticket-filter"
    onSubmit={handleSubmit}
>
    <input
        type="search"
        name="search"
        placeholder="Search title, description or reference..."
        value={filters.search}
        onChange={handleChange}
    />

    <select
        name="categoryId"
        value={filters.categoryId}
        onChange={handleChange}
        disabled={loading}
    >
        <option value="">All categories</option>

        {categories.map((category) => (
            <option
                key={category.id}
                value={category.id}
            >
                {category.name}
            </option>
        ))}
    </select>

    <select
        name="priorityId"
        value={filters.priorityId}
        onChange={handleChange}
        disabled={loading}
    >
        <option value="">All priorities</option>

        {priorities.map((priority) => (
            <option
                key={priority.id}
                value={priority.id}
            >
                {priority.name}
            </option>
        ))}
    </select>

    <select
        name="statusId"
        value={filters.statusId}
        onChange={handleChange}
        disabled={loading}
    >
        <option value="">All statuses</option>

        {statuses.map((status) => (
            <option
                key={status.id}
                value={status.id}
            >
                {status.name}
            </option>
        ))}
    </select>

    <select
        name="dateRange"
        value={filters.dateRange}
        onChange={handleChange}
    >
        <option value="">Any date</option>
        <option value="day">Last 24 hours</option>
        <option value="week">Last 7 days</option>
        <option value="month">Last month</option>
        <option value="year">Last year</option>
    </select>

    <div className="filter-date-group">
        <label htmlFor="createdAfter">
            Created after
        </label>

        <input
            id="createdAfter"
            type="date"
            name="createdAfter"
            value={filters.createdAfter}
            onChange={handleChange}
        />
    </div>

    <div className="filter-date-group">
        <label htmlFor="createdBefore">
            Created before
        </label>

        <input
            id="createdBefore"
            type="date"
            name="createdBefore"
            value={filters.createdBefore}
            onChange={handleChange}
        />
    </div>

    <button
        type="submit"
        className="filter-btn"
    >
        Apply
    </button>

    <button
        type="button"
        className="filter-reset-btn"
        onClick={handleReset}
    >
        Clear
    </button>
</form>
    );
}

export default TicketFilter;