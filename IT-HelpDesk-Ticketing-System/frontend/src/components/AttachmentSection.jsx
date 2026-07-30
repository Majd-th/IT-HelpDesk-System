import { useEffect, useState } from "react";

import {
    getAttachments,
    uploadAttachment,
    downloadAttachment,
    deleteAttachment
} from "../services/ticketAttachmentService";

function AttachmentSection({ ticketId }) {
    const [attachments, setAttachments] =
        useState([]);

    const [selectedFile, setSelectedFile] =
        useState(null);

    const [loading, setLoading] =
        useState(true);

    const [uploading, setUploading] =
        useState(false);

    const [error, setError] =
        useState("");

    const role =
        localStorage.getItem("role");

    const currentUserId = Number(
        localStorage.getItem("userId")
    );

    useEffect(() => {
        if (ticketId) {
            loadAttachments();
        }
    }, [ticketId]);

    async function loadAttachments() {
        try {
            setLoading(true);
            setError("");

            const data =
                await getAttachments(ticketId);

            setAttachments(data);
        } catch (requestError) {
            console.error(
                "Could not load attachments:",
                requestError
            );

            setError(
                "Could not load attachments."
            );
        } finally {
            setLoading(false);
        }
    }

    async function handleUpload(event) {
        event.preventDefault();

        if (!selectedFile) {
            setError(
                "Please select a file first."
            );

            return;
        }

        try {
            setUploading(true);
            setError("");

            await uploadAttachment(
                ticketId,
                selectedFile
            );

            setSelectedFile(null);

            const fileInput =
                document.getElementById(
                    `attachment-file-${ticketId}`
                );

            if (fileInput) {
                fileInput.value = "";
            }

            await loadAttachments();
        } catch (requestError) {
            console.error(
                "Attachment upload failed:",
                requestError
            );

            setError(
                requestError.response?.data
                    ?.message ||
                "Could not upload attachment."
            );
        } finally {
            setUploading(false);
        }
    }

    async function handleDownload(attachment) {
        try {
            setError("");

            await downloadAttachment(
                attachment.id,
                attachment.fileName
            );
        } catch (requestError) {
            console.error(
                "Attachment download failed:",
                requestError
            );

            setError(
                "Could not download attachment."
            );
        }
    }

    async function handleDelete(attachment) {
        const canDelete =
            role === "Admin" ||
            attachment.userId === currentUserId;

        if (!canDelete) {
            setError(
                "You cannot delete this attachment."
            );

            return;
        }

        const confirmed =
            window.confirm(
                `Delete ${attachment.fileName}?`
            );

        if (!confirmed)
            return;

        try {
            setError("");

            await deleteAttachment(
                attachment.id
            );

            await loadAttachments();
        } catch (requestError) {
            console.error(
                "Attachment deletion failed:",
                requestError
            );

            setError(
                requestError.response?.data
                    ?.message ||
                "Could not delete attachment."
            );
        }
    }

    function formatFileSize(bytes) {
        if (!bytes)
            return "0 KB";

        if (bytes < 1024) {
            return `${bytes} bytes`;
        }

        if (bytes < 1024 * 1024) {
            return `${(
                bytes / 1024
            ).toFixed(1)} KB`;
        }

        return `${(
            bytes /
            (1024 * 1024)
        ).toFixed(1)} MB`;
    }

    return (
        <section className="attachment-section">
            <div className="attachment-heading">
                <h2>Attachments</h2>

                <p>
                    Upload supporting files for this
                    ticket.
                </p>
            </div>

            {error && (
                <div className="attachment-error">
                    {error}
                </div>
            )}

            <form
                className="attachment-upload-form"
                onSubmit={handleUpload}
            >
                <input
                    id={`attachment-file-${ticketId}`}
                    type="file"
                    accept=".pdf,.png,.jpg,.jpeg,.doc,.docx,.txt"
                    onChange={(event) =>
                        setSelectedFile(
                            event.target.files?.[0] ||
                            null
                        )
                    }
                />

                <button
                    type="submit"
                    disabled={
                        uploading ||
                        !selectedFile
                    }
                >
                    {uploading
                        ? "Uploading..."
                        : "Upload"}
                </button>
            </form>

            {loading ? (
                <p>Loading attachments...</p>
            ) : attachments.length === 0 ? (
                <p>No attachments found.</p>
            ) : (
                <div className="attachment-list">
                    {attachments.map(
                        (attachment) => {
                            const canDelete =
                                role === "Admin" ||
                                attachment.userId ===
                                    currentUserId;

                            return (
                                <div
                                    key={attachment.id}
                                    className="attachment-item"
                                >
                                    <div className="attachment-info">
                                        <strong>
                                            {
                                                attachment.fileName
                                            }
                                        </strong>

                                        <span>
                                            {formatFileSize(
                                                attachment.fileSize
                                            )}
                                        </span>

                                        <span>
                                            {new Date(
                                                attachment.uploadedDate
                                            ).toLocaleString()}
                                        </span>
                                    </div>

                                    <div className="attachment-actions">
                                        <button
                                            type="button"
                                            onClick={() =>
                                                handleDownload(
                                                    attachment
                                                )
                                            }
                                        >
                                            Download
                                        </button>

                                        {canDelete && (
                                            <button
                                                type="button"
                                                onClick={() =>
                                                    handleDelete(
                                                        attachment
                                                    )
                                                }
                                            >
                                                Delete
                                            </button>
                                        )}
                                    </div>
                                </div>
                            );
                        }
                    )}
                </div>
            )}
        </section>
    );
}

export default AttachmentSection;