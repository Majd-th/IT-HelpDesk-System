import { useEffect, useState } from "react";

import {

    uploadAttachment,
    getAttachments,
    deleteAttachment

} from "../services/attachmentService";

import "../assets/tickets.css";

function AttachmentSection({ ticketId }) {

    const [attachments,setAttachments]=useState([]);

    const [file,setFile]=useState(null);

    useEffect(()=>{

        load();

    },[ticketId]);

    async function load(){

        const data=await getAttachments(ticketId);

        setAttachments(data);

    }

    async function upload(){

        if(!file){

            alert("Choose a file.");

            return;

        }

        await uploadAttachment(ticketId,file);

        setFile(null);

        load();

    }

    async function remove(id){

        if(!window.confirm("Delete attachment?"))

            return;

        await deleteAttachment(id);

        load();

    }

    return(

        <div className="attachment-card">

            <h2>

                📎 Attachments

            </h2>

            <div className="upload-box">

                <input

                    type="file"

                    onChange={(e)=>setFile(e.target.files[0])}

                />

                <button

                    className="upload-btn"

                    onClick={upload}

                >

                    Upload Attachment

                </button>

            </div>

            <div className="attachment-list">

                {

                    attachments.map(a=>(

                        <div

                            key={a.id}

                            className="attachment-item"

                        >

                            <div>

                                <strong>

                                    📄 {a.fileName}

                                </strong>

                                <br/>

                                <small>

                                    {(a.fileSize/1024).toFixed(1)} KB

                                </small>

                            </div>

                            <div>

                                <a

                                    href={`http://localhost:5232/api/TicketAttachment/download/${a.id}`}

                                    target="_blank"

                                    rel="noreferrer"

                                >

                                    <button className="download-btn">

                                        Download

                                    </button>

                                </a>

                                <button

                                    className="delete-btn"

                                    onClick={()=>remove(a.id)}

                                >

                                    Delete

                                </button>

                            </div>

                        </div>

                    ))

                }

            </div>

        </div>

    );

}

export default AttachmentSection;