export class FullereneServer {
    /*#protocol = "https";
    #domain = "localhost";
    #port = "7232";
    #version = "api";
    #controller = "Main"
    
    constructor(baseUrl) {
        if (typeof baseUrl === "string") {
            this.baseUrl = baseUrl;
        } else {
            this.baseUrl = `${this.#protocol}://${this.#domain}:${this.#port}/${this.#version}/${this.#controller}/`;
        }
    }

    async post(url, data) {
        const response = await fetch(`${this.baseUrl}${url}`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(data)
        });
        return response.json();
    }

    async get(url) {
        const response = await fetch(`${this.baseUrl}${url}`);
        return response.json();
    }

    async createFullerenesAndLimitedArea(typeLA, typeF, requestData) {
        return this.post(`create-fullerenes-and-limited-area/${typeLA}/${typeF}`, requestData);
    }

    async createOctotree(idLA) {
        return this.post(`create-octotree-base-on-limited-area/${idLA}`, {});
    }

    async createDensityOfFullerenesInLayers(idLA, seriesFs, numberOfLayers, numberOfDots, excess) {
        return this.post(`create-density-of-fullerenes-in-layers/${idLA}/${seriesFs}/${numberOfLayers}/${numberOfDots}/${excess}`, {});
    }

    async runTestsOnFullerenesCollision(typeLA, typeF, requestData) {
        return this.post(`run-tests-on-fullerenes-collision-in-limited-area/${typeLA}/${typeF}`, requestData);
    }

    async getFullerenesAndLimitedArea(idLA) {
        return this.get(`get-fullerenes-and-limited-area/${idLA}`);
    }*/
}

export default FullereneServer;