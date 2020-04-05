import moment from 'moment';
import Swal from 'sweetalert2'

export const renderStripedTable = (record: any, index: number) => {
    if (index % 2 === 0) {
        return 'default';
    } else {
        return 'striped';
    }
}

export const success = (msg: string) => {
    Swal.fire({
        icon: 'success',
        text: msg
    });
}

export const error = (msg: string) => {
    Swal.fire({
        icon: 'error',
        text: msg
    });
}

export const warning = (msg: string) => {
    Swal.fire({
        icon: 'warning',
        text: msg
    });
}