import { groupBy, Dictionary } from "../utilities/listExtensions";
import { Appointment, Category, Report, User } from "./entities";

export const createReport = (
    appointments: Appointment[],
    categories: Category[],
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
        entries: usersCategories.map(category => ({ category, count: categoryGroups[category.id].length })),
    };

    return report;
}
