
import http from 'k6/http';

const ticketFile = open('./data/6aca50aa-f898-4de2-b540-2be6eb12b7d7.png', 'b');


export default function () {

    const data = {
        file: http.file(ticketFile, 'ticket.png', 'image/png'),
        modelId: '75f5614c-eded-4413-a3d9-c67281e8402e'
      };

    const res = http.post('http://localhost:5257/api/ocr', data);

}
