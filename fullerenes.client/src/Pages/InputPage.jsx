import { useState } from "react";

export default function InputPage() {
    const [formData, setFormData] = useState({
        "Area center X": "", "Area center Y": "", "Area center Z": "", "Area additional parameters": "",
        "Area number of series": "",
        "Fullerene number": "",
        "Fullerene min size": "", "Fullerene max size": "",
        "Shape for fullerene size generation": "", "Scale for fullerene size generation": "",
        "Fullerene praecessio angle": "", "Fullerene nutatio angle": "", "Fullerene proper rotation angle": ""
    });

    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData((prev) => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setLoading(true);

        const requestData = {
            //...formData,
            AreaX: parseFloat(formData["Area center X"]),
            AreaY: parseFloat(formData["Area center Y"]),
            AreaZ: parseFloat(formData["Area center Z"]),
            AreaAdditionalParams: formData["Area additional parameters"].split(",").map(Number),
            NumberOfF: parseInt(formData["Fullerene number"]),
            MinSizeF: parseFloat(formData["Fullerene min size"]),
            MaxSizeF: parseFloat(formData["Fullerene max size"]),
            MaxAlphaF: parseFloat(formData["Fullerene praecessio angle"]),
            MaxBetaF: parseFloat(formData["Fullerene nutatio angle"]),
            MaxGammaF: parseFloat(formData["Fullerene proper rotation angle"]),
            Shape: parseFloat(formData["Shape for fullerene size generation"]),
            Scale: parseFloat(formData["Scale for fullerene size generation"]),
            NumberOfSeries: parseInt(formData["Area number of series"])
        };

        try {
            const response = await fetch(`/api/Main/create-fullerenes-and-limited-area/Sphere/Icosahedron`, {
                method: 'POST',
                headers: { "Content-Type": "application/json" },
                redirect: "follow",
                body: JSON.stringify(requestData)
            });

            const storedIds = JSON.parse(localStorage.getItem("ids")) || [];
            storedIds.push(response);
            localStorage.setItem("ids", JSON.stringify(storedIds));

            console.log(response);

        } catch (error) {
            console.error("Error in request: ", error);
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="Content-Page">
            <h2>Create Limited Area with Fullerenes</h2>
            <form onSubmit={handleSubmit} className="Input-Form">
                {Object.keys(formData).map((key) => (
                    <div key={key} className="Input-Form-Block">
                        <label>{key}:</label>
                        <input type="text" name={key} value={formData[key]} onChange={handleChange} required />
                    </div>
                ))}
                <button
                    type="submit"
                    disabled={loading}
                >
                    { loading ? "Loading..." : "Send" }
                </button>
            </form>
            {loading && <p className="sub-Text">⏳ Пожалуйста, подождите...</p>}
        </div>
    );
}