import React, { useState } from "react";

export default function InputPage() {
    const [formData, setFormData] = useState({
        areaX: 0,
        areaY: 0,
        areaZ: 0,
        areaParams: [0],
        nc: null,
        minSizeF: 0,
        maxSizeF: 0,
        maxAlphaF: 0,
        maxBetaF: 0,
        maxGammaF: 0,
        shape: 0,
        scale: 0,
        series: 0,
        fullereneNumber: 0,
    });

    const [activeOption, setActiveOption] = useState("areaParams");

    const handleChange = (e) => {
        const { name, value } = e.target;
        const parsedValue = parseFloat(value);

        if (name === "areaParams") {
            const parsed = value
                .split(",")
                .map((v) => parseFloat(v.trim()))
                .filter((v) => !isNaN(v));
            setFormData((prev) => ({ ...prev, areaParams: parsed }));
        } else if (name === "nc") {
            setFormData((prev) => ({ ...prev, nc: parsedValue }));
        } else {
            setFormData((prev) => ({ ...prev, [name]: parsedValue }));
        }
    };

    const handleSubmit = async () => {
        const {
            areaX,
            areaY,
            areaZ,
            areaParams,
            nc,
            minSizeF,
            maxSizeF,
            maxAlphaF,
            maxBetaF,
            maxGammaF,
            shape,
            scale,
            series,
            fullereneNumber,
        } = formData;

        const areaAdditionalParams = {
            areaParams: activeOption === "areaParams" ? areaParams : null,
            nc: activeOption === "nc" ? nc : null,
        };

        const body = {
            areaX,
            areaY,
            areaZ,
            areaAdditionalParams,
            minSizeF,
            maxSizeF,
            maxAlphaF,
            maxBetaF,
            maxGammaF,
            shape,
            scale,
        };

        try {
            const response = await fetch(
                `/api/Main/create-fullerenes-and-limited-area/${series}/${fullereneNumber}`,
                {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    redirect: "follow",
                    body: JSON.stringify(body),
                }
            );

            if (!response.ok) throw new Error("Error creating generation");

            const generationId = await response.json();

            const stored = JSON.parse(localStorage.getItem("generations") || "[]");
            stored.push({ generationId, series });
            localStorage.setItem("generations", JSON.stringify(stored));

            alert(`Saved: generationId = ${generationId}, series = ${series}`);
        } catch (err) {
            console.error(err);
            alert("Error while submitting data");
        }
    };

    return (
        <div>
            <h2>Create Fullerenes</h2>

            <input name="series" placeholder="series" onChange={handleChange} />
            <input name="fullereneNumber" placeholder="fullereneNumber" onChange={handleChange} />
            <input name="areaX" placeholder="areaX" onChange={handleChange} />
            <input name="areaY" placeholder="areaY" onChange={handleChange} />
            <input name="areaZ" placeholder="areaZ" onChange={handleChange} />

            <div>
                <label>
                    <input
                        type="radio"
                        checked={activeOption === "areaParams"}
                        onChange={() => setActiveOption("areaParams")}
                    />
                    Use areaParams
                </label>
                {activeOption === "areaParams" && (
                    <input
                        name="areaParams"
                        placeholder="areaParams (e.g. 1.2, 3.4, 5)"
                        onChange={handleChange}
                    />
                )}
            </div>

            <div>
                <label>
                    <input
                        type="radio"
                        checked={activeOption === "nc"}
                        onChange={() => setActiveOption("nc")}
                    />
                    Use nc
                </label>
                {activeOption === "nc" && (
                    <input name="nc" placeholder="nc" onChange={handleChange} />
                )}
            </div>

            <input name="minSizeF" placeholder="minSizeF" onChange={handleChange} />
            <input name="maxSizeF" placeholder="maxSizeF" onChange={handleChange} />
            <input name="maxAlphaF" placeholder="maxAlphaF" onChange={handleChange} />
            <input name="maxBetaF" placeholder="maxBetaF" onChange={handleChange} />
            <input name="maxGammaF" placeholder="maxGammaF" onChange={handleChange} />
            <input name="shape" placeholder="shape" onChange={handleChange} />
            <input name="scale" placeholder="scale" onChange={handleChange} />

            <button onClick={handleSubmit}>Create</button>
        </div>
    );
}
