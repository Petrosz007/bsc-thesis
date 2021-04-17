import { groupBy, Dictionary } from "../utilities/listExtensions";
import { Appointment, Category, Report, User } from "./entities";
import {Interval} from "luxon";

export const createReport = (
    appointments: Appointment[],
    categories: Category[],
    timespan: Interval,
    owner: User,
    customer: User
): Report => {
    const categoryGroups = groupBy(appointments, a => `${a.category.id}`);
    const usersCategories = Dictionary.keys(categoryGroups)
        .map(id => parseInt(id))
        .map(id => categories.find(c => c.id === id) ?? categories[0]);

    const report: Report = {
        owner,
        customer,
        timespan,
        entries: usersCategories.map(category => ({ category, count: categoryGroups[category.id].length })),
    };

    return report;
}
