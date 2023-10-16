
import http from 'k6/http';

const ticketFile = open('./data/6aca50aa-f898-4de2-b540-2be6eb12b7d7.png', 'b');


export default function () {

    const data = {
        file: http.file(ticketFile, 'ticket.png', 'image/png'),
        modelId: '498dd4e9-f54c-47ef-be08-5976caf1e07f'
      };

    const res = http.post('http://localhost:5257/api/ocr', data);

}
