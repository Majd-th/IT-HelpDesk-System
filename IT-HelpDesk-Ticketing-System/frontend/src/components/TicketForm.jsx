import { useState } from "react";
import { useEffect } from "react";

import {

    getCategories,
    getPriorities,
    getStatuses

} from "../services/lookupService";
function TicketForm({

    initialValues={},
    onSubmit

}){

    const [title,setTitle]=useState(initialValues.title||"");
    const [description,setDescription]=useState(initialValues.description||"");
    const [categoryId,setCategoryId]=useState(initialValues.categoryId||"");
    const [priorityId,setPriorityId]=useState(initialValues.priorityId||"");
    const [statusId,setStatusId]=useState(initialValues.statusId||"");
    const [solution,setSolution]=useState(initialValues.solution||"");
    
    const [categories,setCategories]=useState([]);

const [priorities,setPriorities]=useState([]);

const [statuses,setStatuses]=useState([]);
useEffect(()=>{

    loadLookups();

},[]);

async function loadLookups(){

    setCategories(await getCategories());

    setPriorities(await getPriorities());

    setStatuses(await getStatuses());

}

    function submit(e){

        e.preventDefault();

        onSubmit({

            title,
            description,
            categoryId:Number(categoryId),
            priorityId:Number(priorityId),
            statusId:Number(statusId),
            solution

        });

    }

    return(

        <form onSubmit={submit} className="page-card">

            <div className="form-grid">

                <div className="form-group">

                    <label>Title</label>

                    <input

                        value={title}

                        onChange={(e)=>setTitle(e.target.value)}

                    />

                </div>

                <div className="form-group">

                    <label>Category ID</label>
<select

value={categoryId}

onChange={(e)=>setCategoryId(e.target.value)}

>

<option value="">

Choose Category

</option>

{

categories.map(c=>(

<option

key={c.id}

value={c.id}

>

{c.name}

</option>

))

}

</select>

                </div>

                <div className="form-group">

                    <label>Priority ID</label>

                  <select

value={priorityId}

onChange={(e)=>setPriorityId(e.target.value)}

>

<option>

Choose Priority

</option>

{

priorities.map(p=>(

<option

key={p.id}

value={p.id}

>

{p.name}

</option>

))

}

</select>

                </div>

                <div className="form-group">

                    <label>Status ID</label>

               <select

value={statusId}

onChange={(e)=>setStatusId(e.target.value)}

>

<option>

Choose Status

</option>

{

statuses.map(s=>(

<option

key={s.id}

value={s.id}

>

{s.name}

</option>

))

}

</select>

                </div>

                <div className="form-group full-width">

                    <label>Description</label>

                    <textarea

                        value={description}

                        onChange={(e)=>setDescription(e.target.value)}

                    />

                </div>

                <div className="form-group full-width">

                    <label>Solution</label>

                    <textarea

                        value={solution}

                        onChange={(e)=>setSolution(e.target.value)}

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