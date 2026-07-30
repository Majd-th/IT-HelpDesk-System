import { useState } from "react";

import Layout from "../components/Layout";

import "../assets/admin-panel.css";

function AdminPanel() {
    const [activeSection, setActiveSection] = useState("users");

    function renderSection() {
        switch (activeSection) {
            case "users":
                return (
                    <section className="admin-section">
                        <div className="admin-section-header">
                            <div>
                                <h2>Users</h2>
                                <p>
                                    Manage system users and their accounts.
                                </p>
                            </div>

                            <button
                                type="button"
                                className="admin-primary-button"
                            >
                                + Add User
                            </button>
                        </div>

                        <div className="admin-table-card">
                            <table className="admin-table">
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Email</th>
                                        <th>Role</th>
                                        <th>Status</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <tr>
                                        <td>Example User</td>
                                        <td>user@example.com</td>
                                        <td>Employee</td>
                                        <td>
                                            <span className="admin-status active">
                                                Active
                                            </span>
                                        </td>
                                        <td>
                                            <button className="admin-action-button">
                                                Edit
                                            </button>

                                            <button className="admin-action-button danger">
                                                Disable
                                            </button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </section>
                );

            case "roles":
                return (
                    <section className="admin-section">
                        <div className="admin-section-header">
                            <div>
                                <h2>Roles</h2>
                                <p>
                                    View and manage system roles.
                                </p>
                            </div>
                        </div>

                        <div className="admin-card-grid">
                            <div className="admin-setting-card">
                                <h3>Administrator</h3>
                                <p>
                                    Full access to all system features.
                                </p>
                            </div>

                            <div className="admin-setting-card">
                                <h3>Manager</h3>
                                <p>
                                    Manages assignments and ticket workflow.
                                </p>
                            </div>

                            <div className="admin-setting-card">
                                <h3>IT Support Agent</h3>
                                <p>
                                    Works on assigned support tickets.
                                </p>
                            </div>

                            <div className="admin-setting-card">
                                <h3>Employee</h3>
                                <p>
                                    Creates and manages personal tickets.
                                </p>
                            </div>
                        </div>
                    </section>
                );

            case "categories":
                return (
                    <section className="admin-section">
                        <div className="admin-section-header">
                            <div>
                                <h2>Categories</h2>
                                <p>
                                    Manage ticket categories.
                                </p>
                            </div>

                            <button
                                type="button"
                                className="admin-primary-button"
                            >
                                + Add Category
                            </button>
                        </div>

                        <div className="admin-table-card">
                            <table className="admin-table">
                                <thead>
                                    <tr>
                                        <th>Name</th>
                                        <th>Description</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>

                                <tbody>
                                    <tr>
                                        <td>Hardware</td>
                                        <td>
                                            Computer and equipment issues.
                                        </td>
                                        <td>
                                            <button className="admin-action-button">
                                                Edit
                                            </button>

                                            <button className="admin-action-button danger">
                                                Delete
                                            </button>
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                    </section>
                );

            case "priorities":
                return (
                    <section className="admin-section">
                        <div className="admin-section-header">
                            <div>
                                <h2>Priorities</h2>
                                <p>
                                    Manage ticket priority levels.
                                </p>
                            </div>

                            <button
                                type="button"
                                className="admin-primary-button"
                            >
                                + Add Priority
                            </button>
                        </div>

                        <div className="admin-card-grid">
                            <div className="admin-setting-card">
                                <h3>Low</h3>
                            </div>

                            <div className="admin-setting-card">
                                <h3>Medium</h3>
                            </div>

                            <div className="admin-setting-card">
                                <h3>High</h3>
                            </div>

                            <div className="admin-setting-card">
                                <h3>Critical</h3>
                            </div>
                        </div>
                    </section>
                );

            case "statuses":
                return (
                    <section className="admin-section">
                        <div className="admin-section-header">
                            <div>
                                <h2>Statuses</h2>
                                <p>
                                    Manage ticket workflow statuses.
                                </p>
                            </div>

                            <button
                                type="button"
                                className="admin-primary-button"
                            >
                                + Add Status
                            </button>
                        </div>

                        <div className="admin-card-grid">
                            {[
                                "Open",
                                "In Progress",
                                "Pending",
                                "Resolved",
                                "Closed"
                            ].map((status) => (
                                <div
                                    className="admin-setting-card"
                                    key={status}
                                >
                                    <h3>{status}</h3>
                                </div>
                            ))}
                        </div>
                    </section>
                );

            case "settings":
                return (
                    <section className="admin-section">
                        <div className="admin-section-header">
                            <div>
                                <h2>System Settings</h2>
                                <p>
                                    Configure general system settings.
                                </p>
                            </div>
                        </div>

                        <div className="admin-table-card">
                            <form className="admin-settings-form">
                                <div className="admin-form-group">
                                    <label htmlFor="systemName">
                                        System name
                                    </label>

                                    <input
                                        id="systemName"
                                        type="text"
                                        defaultValue="IT HelpDesk"
                                    />
                                </div>

                                <div className="admin-form-group">
                                    <label htmlFor="supportEmail">
                                        Support email
                                    </label>

                                    <input
                                        id="supportEmail"
                                        type="email"
                                        placeholder="support@example.com"
                                    />
                                </div>

                                <div className="admin-form-group">
                                    <label htmlFor="maxAgentTickets">
                                        Maximum active tickets per agent
                                    </label>

                                    <input
                                        id="maxAgentTickets"
                                        type="number"
                                        min="1"
                                        defaultValue="5"
                                    />
                                </div>

                                <button
                                    type="button"
                                    className="admin-primary-button"
                                >
                                    Save Settings
                                </button>
                            </form>
                        </div>
                    </section>
                );

            default:
                return null;
        }
    }

    return (
        <Layout>
            <div className="admin-panel-page">
                <div className="admin-panel-heading">
                    <h1>Admin Settings</h1>

                    <p>
                        Manage users, ticket configuration, and system settings.
                    </p>
                </div>

                <div className="admin-panel-layout">
                    <aside className="admin-panel-menu">
                        <button
                            type="button"
                            className={
                                activeSection === "users"
                                    ? "admin-menu-button active"
                                    : "admin-menu-button"
                            }
                            onClick={() => setActiveSection("users")}
                        >
                            Users
                        </button>

                        <button
                            type="button"
                            className={
                                activeSection === "roles"
                                    ? "admin-menu-button active"
                                    : "admin-menu-button"
                            }
                            onClick={() => setActiveSection("roles")}
                        >
                            Roles
                        </button>

                        <button
                            type="button"
                            className={
                                activeSection === "categories"
                                    ? "admin-menu-button active"
                                    : "admin-menu-button"
                            }
                            onClick={() => setActiveSection("categories")}
                        >
                            Categories
                        </button>

                        <button
                            type="button"
                            className={
                                activeSection === "priorities"
                                    ? "admin-menu-button active"
                                    : "admin-menu-button"
                            }
                            onClick={() => setActiveSection("priorities")}
                        >
                            Priorities
                        </button>

                        <button
                            type="button"
                            className={
                                activeSection === "statuses"
                                    ? "admin-menu-button active"
                                    : "admin-menu-button"
                            }
                            onClick={() => setActiveSection("statuses")}
                        >
                            Statuses
                        </button>

                        <button
                            type="button"
                            className={
                                activeSection === "settings"
                                    ? "admin-menu-button active"
                                    : "admin-menu-button"
                            }
                            onClick={() => setActiveSection("settings")}
                        >
                            System Settings
                        </button>
                    </aside>

                    <div className="admin-panel-content">
                        {renderSection()}
                    </div>
                </div>
            </div>
        </Layout>
    );
}

export default AdminPanel;