import {User} from './user.model';

export interface Medic extends User {
    rank : string;
    specialization : string;
    hospital : string;
}
