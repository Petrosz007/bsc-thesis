import { Report } from "./entities";
import * as pdfMake from "pdfmake/build/pdfmake";
import { TDocumentDefinitions } from "pdfmake/interfaces";

export const downloadReportPdf = (report: Report) => {
    const docDefinition = createPdfDocDefinition(report);
    const fileName = `szamla-${report.customer.name.replaceAll(' ', '_')}-${report.owner.name.replaceAll(' ', '_')}`;
    pdfMake.createPdf(docDefinition).download(fileName);
}

const createPdfDocDefinition = (report: Report) => {
    const totalPrice = report.entries.reduce((acc, x) => 
        acc + x.count * x.category.price
        , 0);

    const numberOfRows = report.entries.length + 2;

    const docDefinition: TDocumentDefinitions = {
        content: [
            {
                text: 'Számla',
                style: 'header',
            },
            {
                columns: [
                    {
                        width: '50%',
                        text: `Vevő: ${report.customer.name}`,
                    },
                    {
                        width: '50%',
                        text:`Kiállító: ${report.owner.name}`,
                        style: 'contractorInfo'
                    },
                ],
            },
            {
                style: 'table',
                // layout: {
                //     fillColor: (rowIndex, _node, _columnIndex) => {
                //         if(rowIndex === 0) return null;
                //         if(rowIndex === numberOfRows - 1) return null;
                //         return (rowIndex % 2 === 1) ? '#F0F0F0' : null;
                //     },
                // },
                layout: 'lightHorizontalLines',
                table: {
                    widths: ['*','auto','auto','auto'],
                    body: [
                        ['Megnevezés', 'Egységár', 'Darab', 'Összesen'].map(text => ({ text, style: 'tableHeaderText' })),
                        ...report.entries.map(e =>
                            [
                                e.category.name,
                                { text:`${e.category.price} Ft`, style: 'tableRightAlign' },
                                { text:`${e.count} db`, style: 'tableRightAlign' },
                                { text:`${e.count * e.category.price} Ft`, style: 'tableRightAlign' },
                            ]
                        ),
                    ]
                },
            },
            { text: `Összesen:    ${totalPrice} Ft`, style: 'tableLastRowText' },
        ],

        styles: {
            header: {
                fontSize: 30,
                margin: [0,0,0,30],
                alignment: 'center',
            },
            contractorInfo: {
                alignment: 'right',
            },
            table: {
                margin: [0,15,0,3],
            },
            tableRightAlign: {
                alignment: 'right',
            },
            tableHeaderText: {
                bold: true,
                alignment: 'left',
            },
            tableLastRowText: {
                fontSize: 13,
                bold: true,
                alignment: 'right',
            }
        }
    };

    return docDefinition;
}