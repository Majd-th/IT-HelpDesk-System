import { useState } from "react";

function TicketFilter({ onFilter }) {

    const [filters, setFilters] = useState({

       search: "",

        categoryId: "",

        priorityId: "",

        statusId: "",

        createdAfter: "",

        createdBefore: ""

    });

    function change(e) {

        setFilters({

            ...filters,

            [e.target.name]: e.target.value

        });

    }

    function submit(e) {

        e.preventDefault();

        onFilter(filters);

    }

    function clear() {

        const empty = {

            search: "",

            categoryId: "",

            priorityId: "",

            statusId: "",

            createdAfter: "",

            createdBefore: ""

        };

        setFilters(empty);

        onFilter(empty);

    }

    return (

        <form onSubmit={submit}>

            <h3>Filters</h3>

           

            <input
                name="categoryId"
                placeholder="Category Id"
                value={filters.categoryId}
                onChange={change}
            />

            <input
                name="priorityId"
                placeholder="Priority Id"
                value={filters.priorityId}
                onChange={change}
            />

            <input
                name="statusId"
                placeholder="Status Id"
                value={filters.statusId}
                onChange={change}
            />

            <label>

                Created After

                <input
                    type="date"
                    name="createdAfter"
                    value={filters.createdAfter}
                    onChange={change}
                />

            </label>

            <label>

                Created Before

                <input
                    type="date"
                    name="createdBefore"
                    value={filters.createdBefore}
                    onChange={change}
                />

            </label>

            <button>

                Filter

            </button>

            <button
                type="button"
                onClick={clear}
            >

                Clear

            </button>

        </form>

    );

}

export default TicketFilter;