import { Report } from "./entities";
import * as pdfMake from "pdfmake/build/pdfmake";
import type { TDocumentDefinitions } from "pdfmake/interfaces";
import {DateTime} from "luxon";

export const downloadReportPdf = (report: Report) => {
    const docDefinition = createPdfDocDefinition(report);
    
    const customerName = report.customer.name.replaceAll(' ', '_');
    const contractorName = report.owner.name.replaceAll(' ', '_');
    const interval = `${report.timespan.start.toISODate()}-${report.timespan.end.toISODate()}`;
        
    const fileName = `szamla-${customerName}-${contractorName}-${interval}`;
    pdfMake.createPdf(docDefinition).download(fileName);
}

const createPdfDocDefinition = (report: Report) => {
    const totalPrice = report.entries.reduce((acc, x) => 
        acc + x.count * x.category.price
        , 0);

    const docDefinition: TDocumentDefinitions = {
        content: [
            {
                text: 'Számla',
                style: 'header',
            },
            {
                text: `${report.timespan.start.toFormat('yyyy.MM.dd')} - ${report.timespan.end.toFormat('yyyy.MM.dd')}`,
                style: 'headerDate',
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
            {
                text: `Kelt: ${DateTime.now().toFormat('yyyy.MM.dd')}`,
                style: 'date',
            },
        ],

        styles: {
            header: {
                fontSize: 30,
                alignment: 'center',
            },
            headerDate: {
                fontSize: 9,
                italics: true,
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
                margin: [0,0,0,30],
            },
            date: {
                alignment: 'right',
            },
        }
    };

    return docDefinition;
}