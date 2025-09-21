FROM node:18-alpine

WORKDIR /app

COPY package*.json ./

RUN npm install

COPY . .

RUN MKDIR -p /app/data

EXPOSE 3000

CMD ["node", "server.js"]

